namespace Ambev.DeveloperEvaluation.Domain.Exceptions;

public static class SalesErrorMessages
{
    public const string SaleNotFound = "Venda não encontrada.";
    public const string SaleAlreadyCancelled = "A venda já está cancelada.";
    public const string SaleItemNotFound = "Item da venda não encontrado.";
    public const string SaleItemAlreadyCancelled = "O item já está cancelado.";

    public const string QuantityAboveLimit = "Não é permitido vender acima de 20 itens idênticos.";
    public const string DiscountNotAllowedBelow4 = "Não é permitido aplicar desconto para quantidades abaixo de 4.";
}
