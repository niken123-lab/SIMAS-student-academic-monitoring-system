Imports MySql.Data.MySqlClient

Module Database

    Public conn As MySqlConnection
    Public Cmd As MySqlCommand
    Public Rd As MySqlDataReader
    Public Da As MySqlDataAdapter
    Public Ds As DataSet


    Public Sub Koneksi()
            Try
                ' Jika koneksi belum dibuat, buat koneksi baru
                If conn Is Nothing Then
                    conn = New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                End If

                ' Jika koneksi tertutup, buka koneksi
                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If

            Catch ex As Exception
                MsgBox("Koneksi gagal: " & ex.Message)
            End Try
        End Sub
    End Module


