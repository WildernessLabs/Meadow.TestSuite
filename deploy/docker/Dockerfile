ARG BASE_REGISTRY=mcr.microsoft.com
ARG SDK_IMAGE=dotnet/sdk
ARG RUNTIME_IMAGE=dotnet/sdk
ARG BASE_TAG=5.0

FROM nginx
# copy over the web stuff
COPY www/ /usr/share/nginx/html/

# install git
RUN apt-get update && \
  apt-get upgrade -y && \
  apt-get install -y git

FROM ${BASE_REGISTRY}/${SDK_IMAGE}:${BASE_TAG} AS build

# clone the TestSuite
RUN git clone https://github.com/WildernessLabs/Meadow.TestSuite.git

# build, publish

WORKDIR /Meadow.TestSuite
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# only use the runtime in the final image
FROM ${BASE_REGISTRY}/${SDK_IMAGE}:${BASE_TAG}
WORKDIR /director
COPY --from=build /director .
ENTRYPOINT ["dotnet", "director.dll"]