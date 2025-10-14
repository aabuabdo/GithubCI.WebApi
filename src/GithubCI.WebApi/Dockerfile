FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# حدد المنفذ الذي سيستمع عليه التطبيق
EXPOSE 8080

# عرّف متغير البيئة حتى .NET يستخدم هذا المنفذ
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GithubCI.WebApi.csproj", "./"]
RUN dotnet restore "GithubCI.WebApi.csproj"
COPY . .
RUN dotnet publish "GithubCI.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GithubCI.WebApi.dll"]
