#scp -i ~/.ssh/id_rsa bin/Release/net6.0/linux-arm/* pi@192.168.0.28:console/
rsync -avh -e ssh bin/Release/net6.0/linux-arm/* pi@192.168.0.28:console/
