Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form20
    Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' Validasi input
            If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox2.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox3.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox4.Text) Then
                MessageBox.Show("Semua kolom wajib diisi (kecuali ID).", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Pastikan koneksi bersih
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            ' Query tanpa id_guru (auto_increment)
            Dim sql As String = "
                INSERT INTO `user` (`nama`, `kelas`, `email`, `password`)
                VALUES (@nama, @kelas, @email, @password);
            "

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@nama", TextBox1.Text.Trim())
                cmd.Parameters.AddWithValue("@kelas", TextBox2.Text.Trim())
                cmd.Parameters.AddWithValue("@email", TextBox3.Text.Trim())
                cmd.Parameters.AddWithValue("@password", TextBox4.Text.Trim())
                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Data wali kelas berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As MySqlException
            MessageBox.Show("Gagal menambahkan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
End Class
