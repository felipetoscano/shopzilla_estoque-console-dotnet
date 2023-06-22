# ShopZilla.Estoque

Projeto ShopZilla responsável por atualizar o estoque e confirmar pedidos.

## Geral

Este projeto faz parte de um conjunto de outros projetos ShopZilla, destinados aos estudos da arquitetura orientada a eventos, Kafka, Entity Framework e execução de processos em segundo plano via BackgroundServices.

O ShopZilla é um projeto que busca simular a efetivação de compras, atualização de estoque e envio de notificações para os clientes através de aplicações independentes, onde aproveitando da arquitetura orientada a eventos, caso algum sistema esteja completamente fora, não vai afetar o conjunto como um todo.

![alt text](https://github.com/felipetoscano/shopzilla_estoque-console-dotnet/blob/main/resources/shopzilla-geral.jpg)

## Aplicação Estoque 

Esta aplicação atualiza o estoque e confirma os pedidos que chegam através do tópico NOVO_PEDIDO onde está inscrito.

Ao confirmar um pedido, podendo aprovar ou recusar de acordo com o estoque disponível, faz a publicação do pedido atualizado no tópico CONFIRMACAO_PEDIDO.

![alt text](https://github.com/felipetoscano/shopzilla_estoque-console-dotnet/blob/main/resources/shopzilla-estoque.jpg)

## Execução

### Para a geração da imagem Docker

A Aplicação, utilizando os recursos mais recentes do .NET 7, possui suporte integrado a geração de imagens Docker.

Basta inserir o comando abaixo na raiz do projeto: 

`
dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
`

## Para a execução do ecossistema

Para a simulação no ambiente local, executar o arquivo docker-compose.yml presente no projeto abaixo:

https://github.com/felipetoscano/shopzilla-scripts-docker

Além disso, garanta que as imagens de todos as aplicações .NET foram geradas.
