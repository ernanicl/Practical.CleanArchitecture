﻿using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Storage.Commands;
using ClassifiedAds.Services.Storage.Constants;
using ClassifiedAds.Services.Storage.Entities;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.EventHandlers;

public class FileEntryCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<FileEntry>>
{
    private readonly Dispatcher _dispatcher;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<OutboxEvent, Guid> _outboxEventRepository;

    public FileEntryCreatedEventHandler(Dispatcher dispatcher,
        ICurrentUser currentUser,
        IRepository<OutboxEvent, Guid> outboxEventRepository)
    {
        _dispatcher = dispatcher;
        _currentUser = currentUser;
        _outboxEventRepository = outboxEventRepository;
    }

    public async Task HandleAsync(EntityCreatedEvent<FileEntry> domainEvent, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(new AddAuditLogEntryCommand
        {
            AuditLogEntry = new AuditLogEntry
            {
                UserId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty,
                CreatedDateTime = domainEvent.EventDateTime,
                Action = "CREATED_FILEENTRY",
                ObjectId = domainEvent.Entity.Id.ToString(),
                Log = domainEvent.Entity.AsJsonString(),
            },
        });

        await _outboxEventRepository.AddOrUpdateAsync(new OutboxEvent
        {
            EventType = EventTypeConstants.FileEntryCreated,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = domainEvent.EventDateTime,
            ObjectId = domainEvent.Entity.Id.ToString(),
            Payload = domainEvent.Entity.AsJsonString(),
            ActivityId = Activity.Current.Id,
        }, cancellationToken);

        await _outboxEventRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
