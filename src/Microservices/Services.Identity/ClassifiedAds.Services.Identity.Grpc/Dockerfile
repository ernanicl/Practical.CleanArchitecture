FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /ClassifiedAds.Microservices

# Copy csproj and restore as distinct layers
COPY ./Common/ClassifiedAds.Application/*.csproj ./Common/ClassifiedAds.Application/
COPY ./Common/ClassifiedAds.CrossCuttingConcerns/*.csproj ./Common/ClassifiedAds.CrossCuttingConcerns/
COPY ./Common/ClassifiedAds.Domain/*.csproj ./Common/ClassifiedAds.Domain/
COPY ./Common/ClassifiedAds.Infrastructure/*.csproj ./Common/ClassifiedAds.Infrastructure/
RUN dotnet restore ./Common/ClassifiedAds.Application/ClassifiedAds.Application.csproj
RUN dotnet restore ./Common/ClassifiedAds.Infrastructure/ClassifiedAds.Infrastructure.csproj

COPY ./Services.Identity/ClassifiedAds.Services.Identity.Api/*.csproj ./Services.Identity/ClassifiedAds.Services.Identity.Api/
RUN dotnet restore ./Services.Identity/ClassifiedAds.Services.Identity.Api/ClassifiedAds.Services.Identity.Api.csproj

COPY ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/*.csproj ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/
RUN dotnet restore ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/ClassifiedAds.Services.Identity.Grpc.csproj

# Copy everything else and build ClassifiedAds
COPY . ./
RUN dotnet publish ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/ClassifiedAds.Services.Identity.Grpc.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /ClassifiedAds.Microservices
COPY --from=build-env /ClassifiedAds.Microservices/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Services.Identity.Grpc.dll"]