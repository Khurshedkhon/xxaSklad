''' <summary>
''' Модель расхода товаров
''' </summary>
Public Class Expense
    Public Property ExpenseId As Integer
    Public Property ExpenseNumber As String
    Public Property ExpenseDate As Date
    Public Property ClientId As Integer
    Public Property ClientName As String
    Public Property ProductId As Integer
    Public Property ProductName As String
    Public Property Quantity As Integer
    Public Property UnitPrice As Decimal
    Public Property TotalAmount As Decimal
    Public Property Notes As String
    Public Property UserId As Integer
    Public Property UserName As String
    Public Property CreatedDate As DateTime

    Public Sub New()
    End Sub

    Public Sub New(expenseId As Integer, expenseNumber As String, expenseDate As Date, clientId As Integer, productId As Integer, quantity As Integer)
        Me.ExpenseId = expenseId
        Me.ExpenseNumber = expenseNumber
        Me.ExpenseDate = expenseDate
        Me.ClientId = clientId
        Me.ProductId = productId
        Me.Quantity = quantity
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0} - {1} ({2})", ExpenseNumber, ProductName, Quantity)
    End Function
End Class
