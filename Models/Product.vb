''' <summary>
''' Модель товара
''' </summary>
Public Class Product
    Public Property ProductId As Integer
    Public Property ProductName As String
    Public Property ProductCode As String
    Public Property CategoryId As Integer
    Public Property CategoryName As String
    Public Property Description As String
    Public Property UnitOfMeasure As String
    Public Property CostPrice As Decimal
    Public Property SellingPrice As Decimal
    Public Property IsActive As Boolean
    Public Property CreatedDate As DateTime
    Public Property UpdatedDate As DateTime
    Public Property CurrentStock As Integer

    Public Sub New()
    End Sub

    Public Sub New(productId As Integer, productName As String, productCode As String, categoryId As Integer)
        Me.ProductId = productId
        Me.ProductName = productName
        Me.ProductCode = productCode
        Me.CategoryId = categoryId
    End Sub

    Public Overrides Function ToString() As String
        Return ProductName
    End Function

    Public Function GetProfit() As Decimal
        Return SellingPrice - CostPrice
    End Function

    Public Function GetProfitMargin() As Decimal
        If SellingPrice = 0 Then Return 0
        Return ((SellingPrice - CostPrice) / SellingPrice) * 100
    End Function
End Class
