﻿using System;

namespace ClassifiedAds.Domain.Entities;

public class OutboxEvent : OutboxEventBase, IAggregateRoot
{
}

public class ArchivedOutboxEvent : OutboxEventBase, IAggregateRoot
{
}

public abstract class OutboxEventBase : Entity<Guid>
{
    public string EventType { get; set; }

    public Guid TriggeredById { get; set; }

    public string ObjectId { get; set; }

    public string Payload { get; set; }

    public bool Published { get; set; }

    public string ActivityId { get; set; }
}
