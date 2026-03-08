FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "Movie2Image.Process.Job/Movie2Image.Process.Job.csproj" && \
	dotnet publish "Movie2Image.Process.Job/Movie2Image.Process.Job.csproj" -c Release --no-restore -o /publish



FROM mcr.microsoft.com/dotnet/runtime:10.0

RUN apt-get update && \
	apt-get install -y curl xz-utils && \
	mkdir -p /tmp/ffmpeg && \
	curl -L --retry 5 --retry-delay 2 -o /tmp/ffmpeg/ffmpeg.tar.xz "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linux64-gpl.tar.xz" && \
	tar -xJf /tmp/ffmpeg/ffmpeg.tar.xz -C /tmp/ffmpeg && \
	install -m 0755 /tmp/ffmpeg/ffmpeg-master-latest-linux64-gpl/bin/ffmpeg /usr/local/bin/ffmpeg && \
	install -m 0755 /tmp/ffmpeg/ffmpeg-master-latest-linux64-gpl/bin/ffprobe /usr/local/bin/ffprobe && \
	rm -rf /tmp/ffmpeg;

WORKDIR /app

ENV TZ="America/Sao_Paulo" \
	RABBITMQ_CONNECTION="" \
	ZIP_PATH="/home/zip_path" \
	FRAMES_PATH="/home/frames_path" \
	QUEUE_LIST="" \
	ERROR_MAX_RETRIES="3" \
	LOAD_SERVICE_URL="http://load-api" \
	AUTH_SERVICE_URL="http://auth-api" \
	CLIENT_ID="process" \
	CLIENT_SECRET=""

COPY --from=build publish/ .

ENTRYPOINT ["dotnet", "Movie2Image.Process.Job.dll"]