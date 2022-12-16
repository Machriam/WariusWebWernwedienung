# WariusWebWernwedienung

### Create Certificate via OpenSSL

```
openssl req -newkey rsa:2048 -x509 -nodes -keyout ./wert.key -new -out ./wert.crt -subj /CN=Wernseher/C=WE/ST=Wernden/L=Wer/OU=Wernwedienung/O=Warius/emailAddress=werde.werde@werde.com/ -reqexts SAN -extensions SAN -config <(cat /etc/ssl/openssl.cnf \
    <(printf '[SAN]\nsubjectAltName=IP:192.168.0.x')) -sha256 -days 3650 -addext basicConstraints=CA:true
```
