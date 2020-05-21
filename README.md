# Build server

```ps
docker build . -f .\LiteServer\Dockerfile -t liteserver
```

# Run server

```ps
docker run liteserver -p 9001:9001
```

# Run client

```ps
dotnet run --project LiteClient 127.0.0.1 9001 5
```