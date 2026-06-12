''' <summary>
''' Модель прихода товаров
''' </summary>
Public Class Income
    Public Property IncomeId As Integer
    Public Property IncomeNumber As String
    Public Property IncomeDate As Date
    Public Property ProductId As Integer
    Public Property ProductName As String
    Public Property Quantity As Integer
    Public Property UnitPrice As Decimal
    Public Property TotalAmount As Decimal
    Public Property SupplierName As String
    Public Property Notes As String
    Public Property UserId As Integer
    Public Property UserName As String
    Public Property CreatedDate As DateTime

    Public Sub New()
    End Sub

    Public Sub New(incomeId As Integer, incomeNumber As String, incomeDate As Date, productId As Integer, quantity As Integer)
        Me.IncomeId = incomeId
        Me.IncomeNumber = incomeNumber
        Me.IncomeDate = incomeDate
        Me.ProductId = productId
        Me.Quantity = quantity
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0} - {1} ({2})", IncomeNumber, ProductName, Quantity)
    End Function
End Class
