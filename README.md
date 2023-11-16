# Лабораторные работы по дисциплине "МИКРОСЕРВИСНАЯ АРХИТЕКТУРА"

UserService предназначен для работы с пользователями и созданию jwt токена.

LabService предназначен для демонстрации работы двух микросервисов и авторизации через jwt token (Bearer).

## Стек:
- .Net 7.0
- Postgres
- Rabbitmq
- Ngnix
- Docker
- Elasticsearch
- Kibana
- Serilog

## Лабораторная с базой данных:
` docker exec -t db pg_dumpall -c -U admin > dump.sql `

` docker exec db psql -h localhost -d laba -U admin -W admin -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE pid <> pg_backend_pid() AND datname = 'laba'" `

` docker exec db psql -h localhost -d postgres -U admin -c "DROP DATABASE laba" `

` docker exec db psql -h localhost -d postgres -U admin -c "CREATE DATABASE laba" `

` cat dump.sql | docker exec -i db psql -h localhost -U admin -d laba `

## Лабораторная с docker swarm:
Manager: docker swarm init

Manager: передать токен

Worker: docker swarm join --token ...

Worker (windows): docker load -i ...tar
Worker (linux): docker load < ...tar

Manager: docker node ls

Manager: docker stack deploy --compose-file=docker-compose.stage.yml stage

Manager: docker service ls