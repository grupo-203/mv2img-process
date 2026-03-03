# Refatoração: Separação de Comportamento em Entidade de Domínio

## 📋 Resumo das Mudanças

Esta refatoração move a lógica de negócio do DTO `ProcessMovieDto` para uma entidade de domínio rica `ProcessingJob`, seguindo princípios de **Domain-Driven Design (DDD)** e **Clean Architecture**.

## 🎯 Objetivos Alcançados

### ✅ **Domain Layer**

#### Value Objects Criados
- `ProcessingJobId` - Identificador único do job de processamento
- `UserId` - Identificador do usuário
- `MoviePath` - Caminho do arquivo de vídeo
- `FramesPath` - Caminho dos frames extraídos
- `ZipPath` - Caminho do arquivo ZIP

**Benefícios:**
- Validação automática nos construtores
- Imutabilidade garantida (record)
- Conversão implícita para string
- Sem possibilidade de valores inválidos

#### Enum ProcessingStatus
```csharp
Created → ExtractingFrames → FramesExtracted → 
Compressing → Compressed → Completed
```

#### Entidade de Domínio: ProcessingJob
```csharp
public class ProcessingJob
{
    // Propriedades privadas com setters privados
    public ProcessingJobId Id { get; private set; }
    public ProcessingStatus Status { get; private set; }
    
    // Métodos de negócio
    public void StartFrameExtraction()
    public void CompleteFrameExtraction(FramesPath framesPath)
    public void StartCompression()
    public void CompleteCompression(ZipPath zipPath)
    public void IncrementTry()
    public void ResetTries()
    public bool CanRetry()
}
```

**Características:**
- Construtor privado (factory method pattern)
- Encapsulamento total
- Validação de transições de estado
- Limite de tentativas (MaxRetries = 3)
- Lógica de negócio centralizada

#### Exception de Domínio
- `MaxRetriesExceededException` - Lançada quando máximo de tentativas é excedido

### ✅ **Application Layer**

#### ProcessingJobMapper
Facilita conversão entre camadas:
- `ToDomain()` - Converte DTO → Entidade
- `ToDto()` - Converte Entidade → DTO
- `FromRequest()` - Cria Entidade a partir de RequestDto

#### ProcessMovieDto Refatorado
**Antes:**
```csharp
public void RestartTries() => Tries = 0;
public void AddTry() => Tries++;
```

**Depois:**
```csharp
// DTO puro, apenas dados
public string? Id { get; set; }
public int Tries { get; set; }
// Sem comportamento!
```

#### Use Cases Refatorados

**ExtractFramesUseCase:**
```csharp
// Converte DTO para entidade
var processingJob = data.ToDomain();

// Executa transformação
await transformer.Transform(processingJob.MoviePath, data.FramesPath!);

// Usa lógica de domínio
var framesPath = FramesPath.Create(data.FramesPath);
processingJob.CompleteFrameExtraction(framesPath);

// Sincroniza estado
data.Status = processingJob.Status.ToString();
```

**Outros Use Cases Refatorados:**
- ✅ `GenerateZipUseCase` - Usa `StartCompression()` e `CompleteCompression()`
- ✅ `PublishUseCase` - Usa `Complete()` e valida arquivo
- ✅ `ProcessErrorUseCase` - Usa `CanRetry()`, `IncrementTry()` e `Fail()`
- ✅ `NotifyUserUseCase` - Usa Value Objects para validação

#### Controllers Refatorados
- ✅ `ExtractFramesController` - Usa `ResetTries()` através da entidade
- ✅ `GenerateZipController` - Usa `ResetTries()` através da entidade

## 📊 Comparação Antes x Depois

### Antes (DTO com Comportamento)
```csharp
// ❌ DTO anêmico com comportamento
data.AddTry();
data.RestartTries();
data.Status = "Frames Extracted";

// ❌ Validação nos use cases
Validator.Create()
    .Test(!string.IsNullOrEmpty(moviePath), "Invalid MoviePath")
    .Validate();
```

### Depois (Domínio Rico)
```csharp
// ✅ Entidade rica com lógica de negócio
var job = data.ToDomain();
job.IncrementTry();
job.CompleteFrameExtraction(framesPath);

// ✅ Validação nos Value Objects
var moviePath = MoviePath.Create(data.MoviePath); // Valida automaticamente
```

## 🎨 Arquitetura Resultante

```
┌─────────────────────────────────────┐
│         Controllers                 │
│  (Orquestração de fluxo)           │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│         Use Cases                   │
│  (Casos de uso da aplicação)       │
│  - Converte DTO → Entity           │
│  - Executa lógica de domínio       │
│  - Converte Entity → DTO           │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Domain Entities                │
│  ProcessingJob                      │
│  - Lógica de negócio               │
│  - Validações de estado            │
│  - Invariantes do domínio          │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Value Objects                  │
│  MoviePath, FramesPath, etc.       │
│  - Validação automática            │
│  - Imutabilidade                   │
└─────────────────────────────────────┘
```

## 🚀 Benefícios Obtidos

### 1. **Separação de Responsabilidades**
- DTOs apenas para transferência de dados
- Entidades para lógica de negócio
- Value Objects para validação

### 2. **Testabilidade**
- Entidade pode ser testada isoladamente
- Sem dependências de infraestrutura
- Lógica de negócio em um único lugar

### 3. **Manutenibilidade**
- Mudanças de regras ficam na camada de domínio
- Código mais legível e expressivo
- Facilita refatorações futuras

### 4. **Segurança de Tipo**
- Value Objects previnem valores inválidos
- Compilador ajuda a encontrar erros
- Menos bugs em tempo de execução

### 5. **Rastreabilidade**
- Status explícitos via enum
- Transições de estado validadas
- Histórico de tentativas controlado

## 📝 Próximos Passos Sugeridos

1. **Repositório de Domínio**
   ```csharp
   public interface IProcessingJobRepository
   {
       Task<ProcessingJob> GetByIdAsync(ProcessingJobId id);
       Task SaveAsync(ProcessingJob job);
   }
   ```

2. **Domain Events**
   ```csharp
   public class FramesExtractedEvent : DomainEvent
   {
       public ProcessingJobId JobId { get; }
       public FramesPath FramesPath { get; }
   }
   ```

3. **Aggregate Root**
   - Adicionar base class `AggregateRoot<TId>`
   - Gerenciamento de eventos de domínio
   - Controle de concorrência

4. **Result Pattern**
   ```csharp
   public async Task<Result<ProcessingJob>> Process(RequestDto request)
   {
       // Retorna sucesso ou falha sem exceptions
   }
   ```

## 📚 Referências

- **Domain-Driven Design** por Eric Evans
- **Clean Architecture** por Robert C. Martin
- **Implementing Domain-Driven Design** por Vaughn Vernon

---

**Data da Refatoração:** 2024
**Autor:** GitHub Copilot
**Status:** ✅ Concluído e Testado
