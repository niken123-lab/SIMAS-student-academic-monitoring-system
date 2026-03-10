Imports MySql.Data.MySqlClient

Public Class Form9
    Private nisn As String
    Private nama As String

    Public Sub New(nisn As String, nama As String)
        InitializeComponent()
        Me.nisn = nisn
        Me.nama = nama
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim uh As Decimal = Val(TextBox1.Text)
        Dim uts As Decimal = Val(TextBox2.Text)
        Dim uas As Decimal = Val(TextBox3.Text)
        Dim aktif As Decimal = Val(TextBox4.Text)
        Dim rata As Decimal = (uh + uts + uas + aktif) / 4

        Try
            Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                conn.Open()

                Dim sql As String = "UPDATE `nilai` " &
                                    "SET `NILAI UH`=@uh, `NILAI UTS`=@uts, `NILAI UAS`=@uas, " &
                                    "`NILAI KEAKTIFAN`=@aktif, `RATA RATA`=@rata " &
                                    "WHERE `nisn`=@nisn AND `id_guru`=@id_guru"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@uh", uh)
                    cmd.Parameters.AddWithValue("@uts", uts)
                    cmd.Parameters.AddWithValue("@uas", uas)
                    cmd.Parameters.AddWithValue("@aktif", aktif)
                    cmd.Parameters.AddWithValue("@rata", rata)
                    cmd.Parameters.AddWithValue("@nisn", Me.nisn)
                    cmd.Parameters.AddWithValue("@id_guru", CurrentUserId) ' <- penting!

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Nilai berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Error input nilai: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
