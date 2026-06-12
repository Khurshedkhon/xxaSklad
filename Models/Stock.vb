''' <summary>
''' Модель остатков на складе
''' </summary>
Public Class Stock
    Public Property StockId As Integer
    Public Property ProductId As Integer
    Public Property ProductName As String
    Public Property Quantity As Integer
    Public Property MinimumLevel As Integer
    Public Property MaximumLevel As Integer
    Public Property LastUpdated As DateTime

    Public Sub New()
    End Sub

    Public Sub New(stockId As Integer, productId As Integer, quantity As Integer)
        Me.StockId = stockId
        Me.ProductId = productId
        Me.Quantity = quantity
    End Sub

    Public Function IsLowStock() As Boolean
        Return Quantity <= MinimumLevel
    End Function

    Public Function IsOverStock() As Boolean
        Return Quantity >= MaximumLevel
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0} ({1})", ProductName, Quantity)
    End Function
End Class
