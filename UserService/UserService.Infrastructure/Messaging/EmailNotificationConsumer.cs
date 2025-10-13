using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Common;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging
{
    public class EmailNotificationConsumer : BackgroundService  
    {
    }
}
