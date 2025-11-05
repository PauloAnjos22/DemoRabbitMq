# 💰 Sistema de Pagamentos com RabbitMQ

## 📋 Descrição

Sistema de pagamentos construído com **Clean Architecture** e **Event-Driven Architecture** utilizando RabbitMQ para mensageria assíncrona. Este projeto foi desenvolvido para praticar os princípios de Clean Architecture, SOLID e padrões de comunicação orientada a eventos.

O sistema processa transações financeiras entre usuários e publica eventos de domínio que são consumidos assincronamente por workers para notificações por email e registro de auditoria.

### 🎯 Funcionalidades Principais

- **Processamento de Pagamentos**: Transferência de fundos entre contas de usuários com transações ACID
- **Arquitetura Orientada a Eventos**: Publicação de eventos via RabbitMQ para processamento desacoplado
- **Notificações por Email**: Serviço assíncrono de email para confirmações de transações
- **Log de Auditoria**: Histórico de transações para conformidade e rastreamento
- **Clean Architecture**: Separação clara de responsabilidades entre camadas
- **Princípios SOLID**: Base de código manutenível e testável
- **Docker Compose**: Serviços containerizados para fácil implantação

## Arquitetura
UserService/
├── 📁 UserService.API (Camada de Apresentação)
│ ├── Controllers/ # Endpoints HTTP (PaymentController, CustomerController)
│ ├── Properties/ # Configurações de lançamento
│ ├── appsettings.json # Configurações da aplicação
│ ├── Program.cs # Ponto de entrada da API
│ └── Dockerfile # Containerização da API
│
├── 📁 UserService.Application (Camada de Aplicação)
│ ├── UseCases/ # Orquestração da lógica de negócio
│ │ ├── PaymentCustomer.cs # Processar pagamentos
│ │ ├── RegisterCustomer.cs # Registrar clientes
│ │ ├── DepositFunds.cs # Realizar depósitos
│ │ ├── GetAllCustomers.cs # Consultar clientes
│ │ └── GetAllCustomersAccount.cs # Consultar contas
│ ├── DTOs/ # Objetos de Transferência de Dados
│ │ ├── Common/ # DTOs compartilhados (ResultResponse)
│ │ ├── Customer/ # DTOs de cliente
│ │ └── Payment/ # DTOs de pagamento
│ ├── Interfaces/ # Abstrações (contratos)
│ │ ├── UseCases/ # Interfaces de casos de uso
│ │ ├── Repositories/ # Interfaces de repositórios
│ │ ├── Services/ # Interfaces de serviços
│ │ └── Messaging/ # Interfaces de mensageria
│ ├── Services/ # Serviços de apoio
│ │ ├── PaymentValidator.cs # Validação de pagamentos
│ │ ├── PaymentEventPublisher.cs # Publicação de eventos
│ │ ├── BankAccountAction.cs # Ações em contas
│ │ └── RegisterValidator.cs # Validação de registro
│ └── Mappings/ # Mapeamento de objetos (AutoMapper)
│
├── 📁 UserService.Domain (Camada de Domínio - Núcleo)
│ ├── Entities/ # Modelos de negócio
│ │ ├── Customer.cs # Entidade Cliente
│ │ ├── BankAccount.cs # Entidade Conta Bancária
│ │ ├── Payment.cs # Entidade Pagamento
│ │ └── TransactionLog.cs # Entidade Log de Transação
│ └── Events/ # Eventos de Domínio
│ ├── CustomerPaymentEvent.cs # Evento de pagamento
│ └── TransactionCompletedEvent.cs # Evento de transação completa
│
├── 📁 UserService.Infrastructure (Camada de Infraestrutura)
│ ├── Persistence/ # Acesso a dados
│ │ ├── ApplicationDbContext.cs # Contexto do EF Core
│ │ └── EfUnitOfWork.cs # Unit of Work Pattern
│ ├── Repositories/ # Implementação de repositórios
│ │ ├── CustomerRepository.cs
│ │ ├── BankAccountRepository.cs
│ │ ├── PaymentRepository.cs
│ │ └── TransactionLogRepository.cs
│ ├── Messaging/ # Mensageria RabbitMQ
│ │ ├── RabbitMqPublisher.cs # Publicador de mensagens
│ │ └── RabbitMqConsumer.cs # Consumidor de mensagens
│ ├── Services/ # Serviços externos
│ │ └── EmailService.cs # Serviço de email (SMTP)
│ ├── Configuration/ # Configurações
│ └── Migrations/ # Migrações do banco de dados
│
└── 📁 UserService.EmailWorker (Background Service)
├── Program.cs # Ponto de entrada do worker
├── appsettings.json # Configurações do worker
├── Dockerfile # Containerização do worker
└── Properties/ # Configurações de lançamento

## Tecnologias Utilizadas

| Tecnologia | Descrição |
|------------|-----------|
| **.NET 8** | Framework para desenvolvimento da aplicação |
| **SQL Server 2022** | Banco de dados relacional |
| **RabbitMQ** | Message Broker para comunicação assíncrona |
| **Entity Framework Core** | ORM para acesso a dados |
| **Docker & Docker Compose** | Containerização e orquestração de serviços |
| **MailTrap** | Serviço de testes de email (sandbox SMTP) |
| **AutoMapper** | Mapeamento automático entre objetos |

## Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- ✅ [Docker Desktop](https://www.docker.com/products/docker-desktop/) - Necessário para executar os containers
- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - Apenas para desenvolvimento local
- ✅ [Git](https://git-scm.com/) - Para clonar o repositório
- ✅ Conta gratuita no [MailTrap](https://mailtrap.io) - Para testes de email

Instalação e Configuração
1. Clone o repositório

git clone https://github.com/seuusuario/DemoRabbitMq.git
cd DemoRabbitMq

2. Configure as variáveis de ambiente
Crie um arquivo .env no diretório raiz:
# Banco de dados
MSSQL_SA_PASSWORD=SuaSenhaForte123!

# MailTrap (https://mailtrap.io)
MAILTRAP_USER=seu_usuario_mailtrap
MAILTRAP_PASS=sua_senha_mailtrap

Nota: Cadastre-se gratuitamente no MailTrap para obter suas credenciais SMTP

3. Execute com Docker Compose
# Build e inicialização de todos os serviços
docker-compose up -d

# Verifique se todos os serviços estão rodando
docker-compose ps

# Visualize os logs
docker-compose logs -f

4. Os serviços estarão disponíveis em:

API: http://localhost:8080
RabbitMQ Management: http://localhost:15672 (usuário: guest / senha: guest)
SQL Server: localhost,1433

5. Migração do banco de dados
O banco de dados será criado automaticamente e as migrações aplicadas na primeira execução.

Como Usar
Endpoints da API