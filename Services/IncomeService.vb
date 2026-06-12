Imports MySql.Data.MySqlClient

''' <summary>
''' Сервис для работы с приходом товаров
''' </summary>
Public Class IncomeService
    ''' <summary>
    ''' Получить все приходы
    ''' </summary>
    Public Shared Function GetAllIncome() As List(Of Income)
        Try
            Dim query As String = "SELECT i.*, p.ProductName, u.FullName as UserName FROM Income i " &
                                 "JOIN Products p ON i.ProductId = p.ProductId " &
                                 "JOIN Users u ON i.UserId = u.UserId " &
                                 "ORDER BY i.IncomeDate DESC"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query)
            Dim incomes As New List(Of Income)

            For Each row As DataRow In dt.Rows
                incomes.Add(MapRowToIncome(row))
            Next

            Return incomes
        Catch ex As Exception
            Throw New Exception("Ошибка при получении приходов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить приход по ID
    ''' </summary>
    Public Shared Function GetIncomeById(incomeId As Integer) As Income
        Try
            Dim query As String = "SELECT i.*, p.ProductName, u.FullName as UserName FROM Income i " &
                                 "JOIN Products p ON i.ProductId = p.ProductId " &
                                 "JOIN Users u ON i.UserId = u.UserId " &
                                 "WHERE i.IncomeId = @incomeId"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@incomeId", incomeId))

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Return MapRowToIncome(dt.Rows(0))
        Catch ex As Exception
            Throw New Exception("Ошибка при получении прихода: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Создать приход товара
    ''' </summary>
    Public Shared Function CreateIncome(incomeDate As Date, productId As Integer, quantity As Integer, 
                                       unitPrice As Decimal, supplierName As String, notes As String, userId As Integer) As Integer
        Try
            Dim incomeNumber As String = GenerateIncomeNumber()
            Dim totalAmount As Decimal = quantity * unitPrice
            
            Dim query As String = "INSERT INTO Income (IncomeNumber, IncomeDate, ProductId, Quantity, UnitPrice, TotalAmount, SupplierName, Notes, UserId) " &
                                 "VALUES (@incomeNumber, @incomeDate, @productId, @quantity, @unitPrice, @totalAmount, @supplierName, @notes, @userId)"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@incomeNumber", incomeNumber),
                New MySqlParameter("@incomeDate", incomeDate),
                New MySqlParameter("@productId", productId),
                New MySqlParameter("@quantity", quantity),
                New MySqlParameter("@unitPrice", unitPrice),
                New MySqlParameter("@totalAmount", totalAmount),
                New MySqlParameter("@supplierName", If(supplierName = "", DBNull.Value, supplierName)),
                New MySqlParameter("@notes", If(notes = "", DBNull.Value, notes)),
                New MySqlParameter("@userId", userId)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            
            ' Обновить остатки
            Dim stockQuery As String = "UPDATE Stock SET Quantity = Quantity + @quantity WHERE ProductId = @productId"
            DatabaseService.ExecuteCommand(stockQuery, 
                New MySqlParameter("@quantity", quantity),
                New MySqlParameter("@productId", productId))
            
            Dim lastIdQuery As String = "SELECT LAST_INSERT_ID()"
            Dim lastId As Object = DatabaseService.ExecuteScalar(lastIdQuery)
            
            Return CInt(lastId)
        Catch ex As Exception
            Throw New Exception("Ошибка при создании прихода: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Удалить приход
    ''' </summary>
    Public Shared Function DeleteIncome(incomeId As Integer) As Boolean
        Try
            ' Получить информацию о приходе перед удалением
            Dim income As Income = GetIncomeById(incomeId)
            If income Is Nothing Then
                Return False
            End If

            ' Уменьшить остатки на количество при удалении прихода
            Dim stockQuery As String = "UPDATE Stock SET Quantity = Quantity - @quantity WHERE ProductId = @productId"
            DatabaseService.ExecuteCommand(stockQuery, 
                New MySqlParameter("@quantity", income.Quantity),
                New MySqlParameter("@productId", income.ProductId))

            ' Удалить запись о приходе
            Dim query As String = "DELETE FROM Income WHERE IncomeId = @incomeId"
            DatabaseService.ExecuteCommand(query, New MySqlParameter("@incomeId", incomeId))
            
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при удалении прихода: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить приходы за период
    ''' </summary>
    Public Shared Function GetIncomeByDateRange(startDate As Date, endDate As Date) As List(Of Income)
        Try
            Dim query As String = "SELECT i.*, p.ProductName, u.FullName as UserName FROM Income i " &
                                 "JOIN Products p ON i.ProductId = p.ProductId " &
                                 "JOIN Users u ON i.UserId = u.UserId " &
                                 "WHERE i.IncomeDate BETWEEN @startDate AND @endDate " &
                                 "ORDER BY i.IncomeDate DESC"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@startDate", startDate),
                New MySqlParameter("@endDate", endDate)
            }
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, parameters)
            Dim incomes As New List(Of Income)

            For Each row As DataRow In dt.Rows
                incomes.Add(MapRowToIncome(row))
            Next

            Return incomes
        Catch ex As Exception
            Throw New Exception("Ошибка при получении приходов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить общую сумму приходов
    ''' </summary>
    Public Shared Function GetTotalIncomeAmount() As Decimal
        Try
            Dim query As String = "SELECT SUM(TotalAmount) as Total FROM Income"
            Dim result As Object = DatabaseService.ExecuteScalar(query)
            
            If result Is Nothing OrElse IsDBNull(result) Then
                Return 0
            End If
            
            Return CDec(result)
        Catch ex As Exception
            Throw New Exception("Ошибка при получении суммы приходов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Генерировать номер прихода
    ''' </summary>
    Private Shared Function GenerateIncomeNumber() As String
        Dim year As String = DateTime.Now.Year.ToString()
        Dim month As String = DateTime.Now.Month.ToString().PadLeft(2, "0"c)
        Dim day As String = DateTime.Now.Day.ToString().PadLeft(2, "0"c)
        
        ' Получить последний номер прихода за день
        Dim query As String = "SELECT COUNT(*) as Count FROM Income WHERE DATE(IncomeDate) = CURDATE()"
        Dim result As Object = DatabaseService.ExecuteScalar(query)
        Dim count As Integer = CInt(result) + 1
        
        Return String.Format("INC-{0}{1}{2}-{3}", year, month, day, count.ToString().PadLeft(4, "0"c))
    End Function

    ''' <summary>
    ''' Маппинг строки данных в объект Income
    ''' </summary>
    Private Shared Function MapRowToIncome(row As DataRow) As Income
        Return New Income With {
            .IncomeId = CInt(row("IncomeId")),
            .IncomeNumber = row("IncomeNumber").ToString(),
            .IncomeDate = CDate(row("IncomeDate")),
            .ProductId = CInt(row("ProductId")),
            .ProductName = row("ProductName").ToString(),
            .Quantity = CInt(row("Quantity")),
            .UnitPrice = CDec(row("UnitPrice")),
            .TotalAmount = CDec(row("TotalAmount")),
            .SupplierName = If(IsDBNull(row("SupplierName")), "", row("SupplierName").ToString()),
            .Notes = If(IsDBNull(row("Notes")), "", row("Notes").ToString()),
            .UserId = CInt(row("UserId")),
            .UserName = row("UserName").ToString()
        }
    End Function
End Class
