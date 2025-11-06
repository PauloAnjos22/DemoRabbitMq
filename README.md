# Sistema de Pagamentos com RabbitMQ

## Descrição

Sistema de pagamentos construído com **Clean Architecture** e **Event-Driven Architecture** utilizando RabbitMQ para mensageria assíncrona. Este projeto foi desenvolvido para praticar os princípios de Clean Architecture, SOLID e padrões de comunicação orientada a eventos.

O sistema processa transações financeiras entre usuários e publica eventos de domínio que são consumidos assincronamente por workers para notificações por email e registro de auditoria.
### Funcionalidades Principais

- **Processamento de Pagamentos**: Transferência de fundos entre contas de usuários com transações ACID
- **Arquitetura Orientada a Eventos**: Publicação de eventos via RabbitMQ para processamento desacoplado
- **Notificações por Email**: Serviço assíncrono de email para confirmações de transações
- **Log de Auditoria**: Histórico de transações para conformidade e rastreamento
- **Clean Architecture**: Separação clara de responsabilidades entre camadas
- **Princípios SOLID**: Base de código manutenível e testável
- **Docker Compose**: Serviços containerizados para fácil implantação
## Arquitetura
> Algumas interfaces e métodos foram omitidos para simplificar a visualização e destacar apenas os principais componentes e suas interações.


<div align="center">
  <img src="./assets/DomainLayer.png" alt="Domain Layer" width="45%">
  <img src="./assets/PresentationLayer.png" alt="Presentation Layer" width="45%">
  <img src="./assets/ApplicationLayer.png" alt="Application Layer" width="45%">
  <img src="./assets/InfrastructureLayer.png" alt="Infrastructure Layer" width="45%">
</div>


## Tecnologias Utilizadas

- .NET 8 - Framework
- SQL Server Management Studio 21 - Banco de dados
- RabbitMQ - Message Broker
- Entity Framework Core - ORM
- Docker & Docker Compose - Containerização
- MailTrap - Serviço de testes de email
- AutoMapper - Mapeamento de objetos 

##  Pré-requisitos
- Docker Desktop instalado e em execução
- .NET 8 SDK (apenas para desenvolvimento)
- Git
- Conta gratuita no [MailTrap](https://mailtrap.io) - Para obter credenciais SMTP de teste
