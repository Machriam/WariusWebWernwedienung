# WariusWebWernwedienung

### Create Certificate via OpenSSL

```
openssl req -newkey rsa:2048 -x509 -nodes -keyout ./wert.key -new -out ./wert.crt -subj /CN=Wernseher/C=WE/ST=Wernden/L=Wer/OU=Wernwedienung/O=Warius/emailAddress=werde.werde@werde.com/ -reqexts SAN -extensions SAN -config <(cat /etc/ssl/openssl.cnf \
    <(printf '[SAN]\nsubjectAltName=IP:192.168.0.x')) -sha256 -days 3650 -addext basicConstraints=CA:true
```

### Add Branch name automatically to commit message
- Add File `prepare-commit-msg` under `.git/hooks` with: 
```
#!/bin/sh
#
# Automatically adds branch name and branch description to every commit message.
#
NAME=$(git branch | grep '*' | sed 's/* //') 
DESCRIPTION=$(git config branch."$NAME".description)

echo "$NAME"': '$(cat "$1") > "$1"
if [ -n "$DESCRIPTION" ] 
then
   echo "" >> "$1"
   echo $DESCRIPTION >> "$1"
fi #
```
