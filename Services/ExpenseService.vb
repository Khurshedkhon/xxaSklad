Imports MySql.Data.MySqlClient

''' <summary>
''' Сервис для работы с расходом товаров
''' </summary>
Public Class ExpenseService
    ''' <summary>
    ''' Получить все расходы
    ''' </summary>
    Public Shared Function GetAllExpense() As List(Of Expense)
        Try
            Dim query As String = "SELECT e.*, c.ClientName, p.ProductName, u.FullName as UserName FROM Expense e " &
                                 "JOIN Clients c ON e.ClientId = c.ClientId " &
                                 "JOIN Products p ON e.ProductId = p.ProductId " &
                                 "JOIN Users u ON e.UserId = u.UserId " &
                                 "ORDER BY e.ExpenseDate DESC"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query)
            Dim expenses As New List(Of Expense)

            For Each row As DataRow In dt.Rows
                expenses.Add(MapRowToExpense(row))
            Next

            Return expenses
        Catch ex As Exception
            Throw New Exception("Ошибка при получении расходов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить расход по ID
    ''' </summary>
    Public Shared Function GetExpenseById(expenseId As Integer) As Expense
        Try
            Dim query As String = "SELECT e.*, c.ClientName, p.ProductName, u.FullName as UserName FROM Expense e " &
                                 "JOIN Clients c ON e.ClientId = c.ClientId " &
                                 "JOIN Products p ON e.ProductId = p.ProductId " &
                                 "JOIN Users u ON e.UserId = u.UserId " &
                                 "WHERE e.ExpenseId = @expenseId"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@expenseId", expenseId))

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Return MapRowToExpense(dt.Rows(0))
        Catch ex As Exception
            Throw New Exception("Ошибка при получении расхода: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Создать расход товара
    ''' </summary>
    Public Shared Function CreateExpense(expenseDate As Date, clientId As Integer, productId As Integer, 
                                        quantity As Integer, unitPrice As Decimal, notes As String, userId As Integer) As Integer
        Try
            ' Проверить доступность товара на складе
            Dim stockQuery As String = "SELECT Quantity FROM Stock WHERE ProductId = @productId"
            Dim stockResult As Object = DatabaseService.ExecuteScalar(stockQuery, New MySqlParameter("@productId", productId))
            
            If stockResult Is Nothing OrElse CInt(stockResult) < quantity Then
                Throw New Exception("Недостаточно товара на складе!")
            End If

            Dim expenseNumber As String = GenerateExpenseNumber()
            Dim totalAmount As Decimal = quantity * unitPrice
            
            Dim query As String = "INSERT INTO Expense (ExpenseNumber, ExpenseDate, ClientId, ProductId, Quantity, UnitPrice, TotalAmount, Notes, UserId) " &
                                 "VALUES (@expenseNumber, @expenseDate, @clientId, @productId, @quantity, @unitPrice, @totalAmount, @notes, @userId)"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@expenseNumber", expenseNumber),
                New MySqlParameter("@expenseDate", expenseDate),
                New MySqlParameter("@clientId", clientId),
                New MySqlParameter("@productId", productId),
                New MySqlParameter("@quantity", quantity),
                New MySqlParameter("@unitPrice", unitPrice),
                New MySqlParameter("@totalAmount", totalAmount),
                New MySqlParameter("@notes", If(notes = "", DBNull.Value, notes)),
                New MySqlParameter("@userId", userId)
            }

            DatabaseService.ExecuteCommand(query, parameters)
            
            ' Уменьшить остатки
            Dim updateStockQuery As String = "UPDATE Stock SET Quantity = Quantity - @quantity WHERE ProductId = @productId"
            DatabaseService.ExecuteCommand(updateStockQuery, 
                New MySqlParameter("@quantity", quantity),
                New MySqlParameter("@productId", productId))
            
            Dim lastIdQuery As String = "SELECT LAST_INSERT_ID()"
            Dim lastId As Object = DatabaseService.ExecuteScalar(lastIdQuery)
            
            Return CInt(lastId)
        Catch ex As Exception
            Throw New Exception("Ошибка при создании расхода: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Удалить расход
    ''' </summary>
    Public Shared Function DeleteExpense(expenseId As Integer) As Boolean
        Try
            ' Получить информацию о расходе перед удалением
            Dim expense As Expense = GetExpenseById(expenseId)
            If expense Is Nothing Then
                Return False
            End If

            ' Вернуть товар на склад
            Dim stockQuery As String = "UPDATE Stock SET Quantity = Quantity + @quantity WHERE ProductId = @productId"
            DatabaseService.ExecuteCommand(stockQuery, 
                New MySqlParameter("@quantity", expense.Quantity),
                New MySqlParameter("@productId", expense.ProductId))

            ' Удалить запись о расходе
            Dim query As String = "DELETE FROM Expense WHERE ExpenseId = @expenseId"
            DatabaseService.ExecuteCommand(query, New MySqlParameter("@expenseId", expenseId))
            
            Return True
        Catch ex As Exception
            Throw New Exception("Ошибка при удалении расхода: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить расходы за период
    ''' </summary>
    Public Shared Function GetExpenseByDateRange(startDate As Date, endDate As Date) As List(Of Expense)
        Try
            Dim query As String = "SELECT e.*, c.ClientName, p.ProductName, u.FullName as UserName FROM Expense e " &
                                 "JOIN Clients c ON e.ClientId = c.ClientId " &
                                 "JOIN Products p ON e.ProductId = p.ProductId " &
                                 "JOIN Users u ON e.UserId = u.UserId " &
                                 "WHERE e.ExpenseDate BETWEEN @startDate AND @endDate " &
                                 "ORDER BY e.ExpenseDate DESC"
            
            Dim parameters As MySqlParameter() = {
                New MySqlParameter("@startDate", startDate),
                New MySqlParameter("@endDate", endDate)
            }
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, parameters)
            Dim expenses As New List(Of Expense)

            For Each row As DataRow In dt.Rows
                expenses.Add(MapRowToExpense(row))
            Next

            Return expenses
        Catch ex As Exception
            Throw New Exception("Ошибка при получении расходов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить расходы по клиенту
    ''' </summary>
    Public Shared Function GetExpenseByClient(clientId As Integer) As List(Of Expense)
        Try
            Dim query As String = "SELECT e.*, c.ClientName, p.ProductName, u.FullName as UserName FROM Expense e " &
                                 "JOIN Clients c ON e.ClientId = c.ClientId " &
                                 "JOIN Products p ON e.ProductId = p.ProductId " &
                                 "JOIN Users u ON e.UserId = u.UserId " &
                                 "WHERE e.ClientId = @clientId " &
                                 "ORDER BY e.ExpenseDate DESC"
            
            Dim dt As DataTable = DatabaseService.ExecuteQuery(query, New MySqlParameter("@clientId", clientId))
            Dim expenses As New List(Of Expense)

            For Each row As DataRow In dt.Rows
                expenses.Add(MapRowToExpense(row))
            Next

            Return expenses
        Catch ex As Exception
            Throw New Exception("Ошибка при получении расходов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Получить общую сумму расходов
    ''' </summary>
    Public Shared Function GetTotalExpenseAmount() As Decimal
        Try
            Dim query As String = "SELECT SUM(TotalAmount) as Total FROM Expense"
            Dim result As Object = DatabaseService.ExecuteScalar(query)
            
            If result Is Nothing OrElse IsDBNull(result) Then
                Return 0
            End If
            
            Return CDec(result)
        Catch ex As Exception
            Throw New Exception("Ошибка при получении суммы расходов: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Генерировать номер расхода
    ''' </summary>
    Private Shared Function GenerateExpenseNumber() As String
        Dim year As String = DateTime.Now.Year.ToString()
        Dim month As String = DateTime.Now.Month.ToString().PadLeft(2, "0"c)
        Dim day As String = DateTime.Now.Day.ToString().PadLeft(2, "0"c)
        
        ' Получить последний номер расхода за день
        Dim query As String = "SELECT COUNT(*) as Count FROM Expense WHERE DATE(ExpenseDate) = CURDATE()"
        Dim result As Object = DatabaseService.ExecuteScalar(query)
        Dim count As Integer = CInt(result) + 1
        
        Return String.Format("EXP-{0}{1}{2}-{3}", year, month, day, count.ToString().PadLeft(4, "0"c))
    End Function

    ''' <summary>
    ''' Маппинг строки данных в объект Expense
    ''' </summary>
    Private Shared Function MapRowToExpense(row As DataRow) As Expense
        Return New Expense With {
            .ExpenseId = CInt(row("ExpenseId")),
            .ExpenseNumber = row("ExpenseNumber").ToString(),
            .ExpenseDate = CDate(row("ExpenseDate")),
            .ClientId = CInt(row("ClientId")),
            .ClientName = row("ClientName").ToString(),
            .ProductId = CInt(row("ProductId")),
            .ProductName = row("ProductName").ToString(),
            .Quantity = CInt(row("Quantity")),
            .UnitPrice = CDec(row("UnitPrice")),
            .TotalAmount = CDec(row("TotalAmount")),
            .Notes = If(IsDBNull(row("Notes")), "", row("Notes").ToString()),
            .UserId = CInt(row("UserId")),
            .UserName = row("UserName").ToString()
        }
    End Function
End Class
