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

![Domain Layer](./assets/DomainLayer.png)

![Presentation Layer](./assets/PresentationLayer.png)

![Application Layer](./assets/ApplicationLayer.png)

![Infrastructure Layer](./assets/InfrastructureLayer.png)


