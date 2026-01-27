using System.Linq;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public class Sale
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string SaleNumber { get; private set; } = default!;
    public DateTime SaleDate { get; private set; }


    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = default!;
    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = default!;

    public SaleStatus Status { get; private set; } = SaleStatus.Active;

    public decimal TotalAmount { get; private set; }
    public decimal TotalDiscount { get; private set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items;

    
    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents;

    protected Sale() { }

    public Sale(
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName)
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;

        CustomerId = customerId;
        CustomerName = customerName;

        BranchId = branchId;
        BranchName = branchName;

        AddEvent(new SaleCreatedEvent(Id, SaleNumber, SaleDate));
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        EnsureActive();
        var item = new SaleItem(Id, productId, productName, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotals();
        AddEvent(new SaleModifiedEvent(Id));
    }
    


    public void UpdateHeader(DateTime saleDate, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        EnsureActive();
        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;

        AddEvent(new SaleModifiedEvent(Id));
    }

    public void ReplaceItems(IEnumerable<SaleItem> newItems)
    {
        EnsureActive();
        _items.Clear();
        _items.AddRange(newItems);
        RecalculateTotals();
        AddEvent(new SaleModifiedEvent(Id));
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
            throw new SalesDomainException(SalesErrorMessages.SaleAlreadyCancelled);

        Status = SaleStatus.Cancelled;

        foreach (var item in _items.Where(i => i.Status == SaleItemStatus.Active))
            item.Cancel();

        RecalculateTotals();
        AddEvent(new SaleCancelledEvent(Id));
    }

    public SaleItem CancelItem(Guid itemId)
    {
        EnsureActive();

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            throw new SalesDomainException(SalesErrorMessages.SaleItemNotFound);

        item.Cancel();
        RecalculateTotals();
        AddEvent(new SaleItemCancelledEvent(Id, itemId));

        return item;
    }

    public void UpdateItem( Guid itemId,int quantity, decimal unitPrice)
    {
        EnsureActive();

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            throw new SalesDomainException(SalesErrorMessages.SaleItemNotFound);

        if (item.Status == SaleItemStatus.Cancelled)
            throw new SalesDomainException(SalesErrorMessages.SaleItemAlreadyCancelled);

        item.UpdateQuantity(quantity);
        item.UpdateUnitPrice(unitPrice);

        RecalculateTotals();
        AddEvent(new SaleModifiedEvent(Id));
    }


    public void SyncItems(IEnumerable<SaleItemSyncData> incoming)
    {
        EnsureActive();

        var incomingList = incoming.ToList();

        foreach (var it in incomingList)
        {
            if (it.Id.HasValue)
            {
                UpdateItem(it.Id.Value, it.Quantity, it.UnitPrice);
            }
            else
            {
                AddItem(it.ProductId, it.ProductName, it.Quantity, it.UnitPrice);
            }
        }

        var incomingIds = incomingList
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToHashSet();

        foreach (var existing in _items.Where(x => x.Status == SaleItemStatus.Active).ToList())
        {
            if (!incomingIds.Contains(existing.Id) &&
                !incomingList.Any(n => !n.Id.HasValue && n.ProductId == existing.ProductId))
            {
                existing.Cancel();
            }
        }

        RecalculateTotals();
        AddEvent(new SaleModifiedEvent(Id));
    }



    public void ClearEvents() => _domainEvents.Clear();

    private void RecalculateTotals()
    {
        TotalAmount = _items.Where(i => i.Status == SaleItemStatus.Active).Sum(i => i.TotalAmount);
        TotalDiscount = _items.Where(i => i.Status == SaleItemStatus.Active).Sum(i => i.DiscountValue);
    }

    private void EnsureActive()
    {
        if (Status == SaleStatus.Cancelled)
            throw new SalesDomainException(SalesErrorMessages.SaleAlreadyCancelled);
    }

    private void AddEvent(object evt) => _domainEvents.Add(evt);
}
