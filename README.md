# Movie2Image.Process - Robô de Processamento de Vídeos

## 📋 Visão Geral

O **Movie2Image.Process** é um robô de processamento assíncrono que transforma arquivos de vídeo em imagens individuais (frames), compacta o resultado em um arquivo ZIP e realiza o upload para armazenamento. O sistema é construído seguindo princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, utilizando uma arquitetura baseada em filas de mensagens para processamento distribuído e resiliente.

### ✨ Funcionalidades Principais

- 🎬 **Extração de Frames**: Converte arquivos de vídeo em sequências de imagens usando FFmpeg
- 📦 **Compactação**: Gera arquivos ZIP otimizados com os frames extraídos
- ☁️ **Upload**: Publica os arquivos processados para armazenamento
- 🔄 **Processamento Assíncrono**: Utiliza filas RabbitMQ para processamento distribuído
- 🛡️ **Resiliência**: Sistema de retry automático com Dead Letter Queue
- 📊 **Monitoramento**: Instrumentalização com Prometheus
- 📧 **Notificações**: Envio de e-mails para usuários

### 🔄 Fluxo de Filas (RabbitMQ)

**Pipeline de Processamento Completo:**

```
📥 carregamentos.video.request 
    → 🎬 extract-frames 
    → 📦 generate-zip 
    → ☁️ publish 
    → ✅ carregamentos.video.finished
```

**Tratamento de Erros:**

```
❌ Erro em qualquer etapa 
    → 🔄 Retry (até 3 vezes)
    → 📮 dead-letter 
    → ⚠️ carregamentos.video.error
```

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação clara de responsabilidades em camadas:

```
┌─────────────────────────────────────────────────────────┐
│                    Movie2Image.Process.Job              │
│                    (Entry Point / Host)                 │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│              Movie2Image.Process.Controller             │
│           (Controladores de Fila / Adapters)            │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│             Movie2Image.Process.Application             │
│              (Use Cases / Application Logic)            │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│               Movie2Image.Process.Domain                │
│          (Entidades, Value Objects, Regras de Negócio)  │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                 Camadas de Infraestrutura               │
│  • Queue (RabbitMQ)  • Storage  • Media (FFmpeg)        │
│  • Mail  • Logging  • ExternalServices  • Zip           │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 Componentes do Sistema

### 🎯 Camada de Domínio (`Movie2Image.Process.Domain`)

**Responsabilidade**: Contém as regras de negócio puras e o modelo de domínio.

#### Entidade Principal: `ProcessingJob`

A entidade rica que gerencia o ciclo de vida do processamento:

```csharp
public class ProcessingJob
{
    public ProcessingJobId Id { get; private set; }
    public UserId UserId { get; private set; }
    public MoviePath MoviePath { get; private set; }
    public FramesPath? FramesPath { get; private set; }
    public ZipPath? TempZipPath { get; private set; }
    public ZipPath? ZipPath { get; private set; }
    public ProcessingStatus Status { get; private set; }
    public int Tries { get; private set; }
    
    // Métodos de comportamento
    public void StartFrameExtraction()
    public void CompleteFrameExtraction(FramesPath framesPath)
    public void StartCompression()
    public void CompleteCompression(ZipPath zipPath)
    public void Complete()
    public void Fail(Exception exception)
    public void IncrementTry()
    public void ResetTries()
    public bool CanRetry()
}
```

#### Value Objects

Objetos de valor imutáveis com validação integrada:

- **ProcessingJobId**: Identificador único do job
- **UserId**: Identificador do usuário
- **MoviePath**: Caminho do arquivo de vídeo
- **FramesPath**: Caminho dos frames extraídos
- **ZipPath**: Caminho do arquivo ZIP

#### Enum: `ProcessingStatus`

Estados do processamento:

```
Created → ExtractingFrames → FramesExtracted → 
Compressing → Compressed → Completed
```

---

### 🧩 Camada de Aplicação (`Movie2Image.Process.Application`)

**Responsabilidade**: Orquestra os casos de uso e a lógica de aplicação.

#### Use Cases

1. **ExtractFramesUseCase**: Extrai frames do vídeo usando FFmpeg
2. **GenerateZipUseCase**: Compacta os frames em arquivo ZIP
3. **PublishUseCase**: Realiza upload do arquivo ZIP
4. **ProcessErrorUseCase**: Gerencia erros e retries
5. **NotifyUserUseCase**: Envia notificações por e-mail

#### Services

- **FramesPathSetService**: Define caminhos para frames
- **TempZipPathSetService**: Define caminhos temporários para ZIP
- **ZipPathSetService**: Define caminhos finais para ZIP
- **TempCleanService**: Limpeza de arquivos temporários

#### DTOs (Data Transfer Objects)

- **ProcessMovieDto**: Dados de processamento
- **RequestDto**: Requisição inicial
- **PublishDataDto**: Dados de publicação
- **ZipDataDto**: Dados de compactação
- **UserDto**: Dados do usuário

---

### 🎮 Camada de Controle (`Movie2Image.Process.Controller`)

**Responsabilidade**: Adaptadores que conectam as filas aos casos de uso.

#### Controladores de Fila

1. **ProcessMovieController**
   - Escuta fila: `carregamentos.video.request`
   - Recebe requisições iniciais de processamento
   - Publica para: `extract-frames`

2. **ExtractFramesController**
   - Escuta fila: `extract-frames`
   - Executa extração de frames com FFmpeg
   - **Sucesso**: Publica para `generate-zip`
   - **Erro**: Retry até 3x, depois vai para `dead-letter`

3. **GenerateZipController**
   - Escuta fila: `generate-zip`
   - Executa compactação de frames em arquivo ZIP
   - **Sucesso**: Publica para `publish`
   - **Erro**: Retry até 3x, depois vai para `dead-letter`

4. **PublishController**
   - Escuta fila: `publish`
   - Executa upload do arquivo para armazenamento
   - Notifica serviço externo (Load-API)
   - **Sucesso**: Publica para `carregamentos.video.finished`
   - **Erro**: Retry até 3x, depois vai para `dead-letter`

5. **DeadLetterController**
   - Escuta fila: `dead-letter`
   - Processa mensagens que falharam após 3 tentativas
   - Publica para: `carregamentos.video.error`
   - Notifica usuários sobre falhas permanentes

---

## 🔄 Fluxo de Processamento

### Arquitetura de Filas

```mermaid
graph TB
    A[carregamentos.video.request] -->|ProcessMovieController| B[extract-frames]
    B -->|ExtractFramesController| C[generate-zip]
    C -->|GenerateZipController| D[publish]
    D -->|PublishController| E[carregamentos.video.finished]
    E --> F[✅ Processamento Concluído]
    
    B -.Erro 1ª tentativa.-> B
    B -.Erro 2ª tentativa.-> B
    B -.Erro 3ª tentativa.-> G[dead-letter]
    
    C -.Erro 1ª tentativa.-> C
    C -.Erro 2ª tentativa.-> C
    C -.Erro 3ª tentativa.-> G
    
    D -.Erro 1ª tentativa.-> D
    D -.Erro 2ª tentativa.-> D
    D -.Erro 3ª tentativa.-> G
    
    G -->|DeadLetterController| H[carregamentos.video.error]
    H --> I[❌ Notificação de Falha]
    
    style A fill:#4CAF50,stroke:#2E7D32,color:#fff
    style E fill:#2196F3,stroke:#1565C0,color:#fff
    style H fill:#F44336,stroke:#C62828,color:#fff
    style G fill:#FF9800,stroke:#E65100,color:#fff
```

### Fluxo de Sucesso

```
carregamentos.video.request 
    ↓
extract-frames 
    ↓
generate-zip 
    ↓
publish 
    ↓
carregamentos.video.finished
```

### Fluxo de Erro (com Retry)

```
Qualquer fila de processamento
    ↓
❌ Erro na 1ª tentativa → Retry (volta para mesma fila)
    ↓
❌ Erro na 2ª tentativa → Retry (volta para mesma fila)
    ↓
❌ Erro na 3ª tentativa → dead-letter
    ↓
carregamentos.video.error
```

### Detalhamento do Fluxo

#### 1. **Recepção** (`carregamentos.video.request`)
- Requisição inicial com dados do vídeo
- ProcessMovieController valida dados
- Encaminha para fila `extract-frames`

#### 2. **Extração de Frames** (`extract-frames`)
- ExtractFramesController executa ExtractFramesUseCase
- FFmpeg extrai frames do vídeo
- Frames salvos em diretório temporário
- Status atualizado para `FramesExtracted`
- **Sucesso**: Encaminha para `generate-zip`
- **Erro**: Retry até 3x, depois vai para `dead-letter`

#### 3. **Compactação** (`generate-zip`)
- GenerateZipController executa GenerateZipUseCase
- Frames compactados em arquivo ZIP
- ZIP salvo em diretório temporário
- Status atualizado para `Compressed`
- **Sucesso**: Encaminha para `publish`
- **Erro**: Retry até 3x, depois vai para `dead-letter`

#### 4. **Publicação** (`publish`)
- PublishController executa PublishUseCase
- Upload do arquivo ZIP para armazenamento
- Notificação ao serviço Load-API
- Limpeza de arquivos temporários
- Status atualizado para `Completed`
- **Sucesso**: Encaminha para `carregamentos.video.finished`
- **Erro**: Retry até 3x, depois vai para `dead-letter`

#### 5. **Conclusão** (`carregamentos.video.finished`)
- Mensagem de sucesso publicada
- Sistema externo notificado
- Processamento finalizado com sucesso

#### 6. **Tratamento de Falhas** (`dead-letter` → `carregamentos.video.error`)
- DeadLetterController processa mensagens que falharam após 3 tentativas
- Mensagem enviada para `carregamentos.video.error`
- Notificação ao usuário sobre falha
- Logs detalhados do erro para análise

---

## 🔧 Camadas de Infraestrutura

### 📨 Queue (`Movie2Image.Process.Queue`)

**Tecnologia**: RabbitMQ

**Filas Utilizadas**:

#### Filas de Processamento
- **`carregamentos.video.request`**: Fila de entrada inicial - recebe requisições de processamento
- **`extract-frames`**: Fila de extração de frames do vídeo
- **`generate-zip`**: Fila de compactação de frames em arquivo ZIP
- **`publish`**: Fila de publicação do arquivo processado

#### Filas de Notificação
- **`carregamentos.video.finished`**: Fila de conclusão bem-sucedida
- **`carregamentos.video.error`**: Fila de notificação de erros

#### Fila de Controle
- **`dead-letter`**: Fila intermediária para mensagens que falharam após máximo de tentativas

**Configuração**: Connection string via variável de ambiente `RABBITMQ_CONNECTION`

**Estratégia de Retry**: 
- Máximo de 3 tentativas por fila de processamento
- Após 3 falhas, mensagem vai para `dead-letter`
- `DeadLetterController` encaminha para `carregamentos.video.error`

#### Mapeamento Controlador → Fila

| Controlador | Fila de Entrada | Fila de Saída (Sucesso) | Fila de Saída (Erro após 3x) |
|-------------|-----------------|-------------------------|------------------------------|
| ProcessMovieController | `carregamentos.video.request` | `extract-frames` | N/A |
| ExtractFramesController | `extract-frames` | `generate-zip` | `dead-letter` |
| GenerateZipController | `generate-zip` | `publish` | `dead-letter` |
| PublishController | `publish` | `carregamentos.video.finished` | `dead-letter` |
| DeadLetterController | `dead-letter` | `carregamentos.video.error` | N/A |

### 🎬 Media (`Movie2Image.Process.Media`)

**Tecnologia**: FFmpeg

**Funcionalidades**:
- Extração de frames de vídeos
- Suporte a múltiplos formatos de vídeo
- Configuração de qualidade e taxa de frames

### 📦 Storage (`Movie2Image.Process.Storage`)

**Funcionalidades**:
- Gerenciamento de arquivos locais
- Upload de arquivos processados
- Limpeza de arquivos temporários

### 🗜️ Zip (`Movie2Image.Process.Zip`)

**Tecnologia**: DotNetZip

**Funcionalidades**:
- Compactação de diretórios
- Otimização de tamanho
- Validação de integridade

### 📧 Mail (`Movie2Image.Process.Mail`)

**Tecnologia**: System.Net.Mail

**Funcionalidades**:
- Envio de notificações por e-mail
- Templates de e-mail
- Notificações de sucesso e falha

### 🔌 ExternalServices (`Movie2Image.Process.ExternalServices`)

**Serviços Integrados**:

1. **LoadService**: 
   - Notifica conclusão de processamento
   - Endpoint: `{LOAD_SERVICE_URL}/api/process/finish`

2. **AuthService**: 
   - Autenticação de serviço a serviço
   - Fluxo: Client Credentials OAuth2

### 📊 Instrumentalization (`Movie2Image.Process.Instrumentalization`)

**Tecnologia**: Prometheus

**Métricas Coletadas**:
- Contadores de mensagens processadas
- Tempos de processamento
- Taxa de erros e retries
- Utilização de recursos

### 📝 Logging (`Movie2Image.Process.Logging`)

**Funcionalidades**:
- Logging estruturado
- Formatação customizada para console
- Integração com Application Insights (opcional)
- Níveis: Debug, Information, Warning, Error, Critical

---

## ⚙️ Configuração

### Variáveis de Ambiente

```bash
# Timezone
TZ=America/Sao_Paulo

# RabbitMQ
RABBITMQ_CONNECTION=amqp://user:password@rabbitmq:5672/vhost

# Paths
ZIP_PATH=/home/zip_path
FRAMES_PATH=/home/frames_path

# Filas RabbitMQ
# Filas de processamento
QUEUE_REQUEST=carregamentos.video.request
QUEUE_EXTRACT_FRAMES=extract-frames
QUEUE_GENERATE_ZIP=generate-zip
QUEUE_PUBLISH=publish
# Filas de notificação
QUEUE_FINISHED=carregamentos.video.finished
QUEUE_ERROR=carregamentos.video.error
# Fila de controle
QUEUE_DEAD_LETTER=dead-letter

# Retry Configuration
ERROR_MAX_RETRIES=3

# External Services
LOAD_SERVICE_URL=http://load-api
AUTH_SERVICE_URL=http://auth-api
CLIENT_ID=process
CLIENT_SECRET=your-secret-here

# Email (opcional)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@domain.com
SMTP_PASSWORD=your-password
EMAIL_FROM=noreply@domain.com

# Prometheus (opcional)
PROMETHEUS_PORT=9090
```

### Arquivo de Configuração (appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "RabbitMQ": {
    "ConnectionString": "#{RABBITMQ_CONNECTION}#",
    "Queues": {
      "ProcessMovie": "carregamentos.video.request",
      "ExtractFrames": "extract-frames",
      "GenerateZip": "generate-zip",
      "Publish": "publish",
      "DeadLetter": "dead-letter",
      "Finished": "carregamentos.video.finished",
      "Error": "carregamentos.video.error"
    }
  },
  "FFmpeg": {
    "ExecutablePath": "/usr/local/bin/ffmpeg",
    "ThreadCount": 4
  },
  "Processing": {
    "MaxRetries": 3,
    "FramesPath": "/home/frames_path",
    "ZipPath": "/home/zip_path"
  }
}
```

---

## 🐳 Deploy com Docker

### Build da Imagem

```bash
docker build -t movie2image-process:latest .
```

### Executar Container

```bash
docker run -d \
  --name movie2image-process \
  -e RABBITMQ_CONNECTION="amqp://user:password@rabbitmq:5672/vhost" \
  -e ZIP_PATH="/app/data/zip" \
  -e FRAMES_PATH="/app/data/frames" \
  -e LOAD_SERVICE_URL="http://load-api" \
  -e AUTH_SERVICE_URL="http://auth-api" \
  -e CLIENT_ID="process" \
  -e CLIENT_SECRET="your-secret" \
  -v /host/data:/app/data \
  movie2image-process:latest
```

### Docker Compose

```yaml
version: '3.8'

services:
  movie2image-process:
    image: movie2image-process:latest
    environment:
      - TZ=America/Sao_Paulo
      - RABBITMQ_CONNECTION=amqp://user:password@rabbitmq:5672/
      - ZIP_PATH=/app/data/zip
      - FRAMES_PATH=/app/data/frames
      - LOAD_SERVICE_URL=http://load-api
      - AUTH_SERVICE_URL=http://auth-api
      - CLIENT_ID=process
      - CLIENT_SECRET=${CLIENT_SECRET}
    volumes:
      - ./data:/app/data
    depends_on:
      - rabbitmq
    restart: unless-stopped

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      - RABBITMQ_DEFAULT_USER=user
      - RABBITMQ_DEFAULT_PASS=password
```

---

## 💻 Desenvolvimento

### Pré-requisitos

- .NET 10.0 SDK
- FFmpeg instalado no PATH
- RabbitMQ Server (local ou Docker)
- IDE: Visual Studio 2022, VS Code ou Rider

### Estrutura de Pastas

```
Movie2Image.Process/
├── Movie2Image.Process.Domain/          # Entidades e regras de negócio
├── Movie2Image.Process.Application/     # Casos de uso
├── Movie2Image.Process.Controller/      # Controladores de fila
├── Movie2Image.Process.Job/            # Entry point da aplicação
├── Movie2Image.Process.Queue/          # Infraestrutura de filas
├── Movie2Image.Process.Media/          # Processamento de vídeo
├── Movie2Image.Process.Storage/        # Gerenciamento de arquivos
├── Movie2Image.Process.Zip/            # Compactação de arquivos
├── Movie2Image.Process.Mail/           # Envio de e-mails
├── Movie2Image.Process.ExternalServices/ # Integração com APIs
├── Movie2Image.Process.Logging/        # Sistema de logs
├── Movie2Image.Process.Instrumentalization/ # Métricas
├── Movie2Image.Process.Domain.Test/    # Testes de domínio
└── Movie2Image.Process.Application.Test/ # Testes de aplicação
```

### Executar Localmente

```bash
# 1. Restaurar dependências
dotnet restore

# 2. Compilar solução
dotnet build

# 3. Executar testes
dotnet test

# 4. Executar aplicação
cd Movie2Image.Process.Job
dotnet run
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test --filter "FullyQualifiedName~ProcessingJob"

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Adicionar Nova Fila de Processamento

1. **Criar Interface no Application/Ports/Input/Controller**
```csharp
public interface IMyNewController
{
    Task Process(ProcessMovieDto data);
}
```

2. **Implementar Controller**
```csharp
public class MyNewController : IMyNewController
{
    public async Task Process(ProcessMovieDto data)
    {
        // Implementação
    }
}
```

3. **Registrar no Program.cs**
```csharp
await services.GetRequiredService<IQueueList>()
    .Add<IMyNewController>(queues.MyNewQueue)
    .Start();
```

---

## 🧪 Testes

### Cobertura de Testes

- ✅ Testes Unitários de Domínio
- ✅ Testes de Casos de Uso
- ✅ Testes de Value Objects
- ✅ Testes de Validação

### Executar Suite Completa

```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 📊 Monitoramento

### Métricas Prometheus

Endpoint: `http://localhost:9090/metrics`

**Métricas Disponíveis**:
- `movie2image_messages_processed_total`: Total de mensagens processadas
- `movie2image_processing_duration_seconds`: Duração do processamento
- `movie2image_errors_total`: Total de erros
- `movie2image_retries_total`: Total de retries

### Health Check

```bash
# Verificar se o serviço está respondendo
curl http://localhost:8080/health
```

---

## 🔒 Segurança

- ✅ Credenciais em variáveis de ambiente (nunca em código)
- ✅ Autenticação OAuth2 para serviços externos
- ✅ Validação de entrada com Value Objects
- ✅ Logs sem informações sensíveis
- ✅ Timeout em operações de I/O

---

## 🚀 Performance

### Otimizações Implementadas

- ⚡ Processamento assíncrono com filas
- ⚡ Paralelização de extração de frames (FFmpeg multi-thread)
- ⚡ Compactação otimizada com DotNetZip
- ⚡ Cleanup automático de arquivos temporários
- ⚡ Connection pooling para RabbitMQ

### Capacidade

- **Throughput**: ~100 vídeos/hora (dependendo do hardware)
- **Tamanho máximo de vídeo**: Limitado pelo espaço em disco
- **Formatos suportados**: Todos os formatos suportados pelo FFmpeg

---

## 🐛 Troubleshooting

### Problemas Comuns

1. **FFmpeg não encontrado**
   ```bash
   # Verificar instalação
   which ffmpeg
   # Instalar no Ubuntu/Debian
   apt-get install ffmpeg
   ```

2. **Erro de conexão RabbitMQ**
   ```bash
   # Verificar se RabbitMQ está rodando
   docker ps | grep rabbitmq
   # Testar conexão
   telnet localhost 5672
   ```

3. **Espaço em disco insuficiente**
   ```bash
   # Verificar espaço
   df -h
   # Limpar arquivos temporários
   rm -rf /home/frames_path/* /home/zip_path/*
   ```

---

## 📚 Referências

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [FFmpeg Documentation](https://ffmpeg.org/documentation.html)
- [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)

---

## 👥 Contribuição

Para contribuir com o projeto:

1. Faça um fork do repositório
2. Crie uma branch para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença especificada no arquivo [LICENSE](LICENSE).

---

**Made with ☕ by FIAP Students**
