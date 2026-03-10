Imports MySql.Data.MySqlClient

Public Class Form21
    Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")

    ' 🔹 Variabel untuk menyimpan ID guru (dikirim dari Form19)
    Public Property IdGuru As Integer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' 🔹 Validasi input
            If IdGuru = 0 Then
                MessageBox.Show("ID Guru tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox2.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox3.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox4.Text) Then
                MessageBox.Show("Semua kolom wajib diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' 🔹 Buka koneksi database
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            ' 🔹 Query update data wali kelas
            Dim sql As String = "
                UPDATE `user`
                SET 
                    `nama` = @nama,
                    `kelas` = @kelas,
                    `email` = @email,
                    `password` = @password
                WHERE `id_guru` = @id_guru;
            "

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@id_guru", IdGuru)
                cmd.Parameters.AddWithValue("@nama", TextBox1.Text.Trim())
                cmd.Parameters.AddWithValue("@kelas", TextBox2.Text.Trim())
                cmd.Parameters.AddWithValue("@email", TextBox3.Text.Trim())
                cmd.Parameters.AddWithValue("@password", TextBox4.Text.Trim())

                Dim rows = cmd.ExecuteNonQuery()

                If rows > 0 Then
                    MessageBox.Show("Data wali kelas berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Tidak ada data yang diperbarui.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using

            ' 🔹 Tutup form setelah update
            Me.Close()

        Catch ex As MySqlException
            MessageBox.Show("Gagal memperbarui data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub Form21_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
