FROM mono
MAINTAINER w2w
RUN mkdir /app
COPY . /app
# RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF; \
#		echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | tee /etc/apt/sources.list.d/mono-official-stable.list; \
#		apt update

# RUN apt install mono-devel

# RUN apt-get -y install git
