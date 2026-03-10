Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form7
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")

        Try
            conn.Open()

            ' Gunakan backtick (`) pada kolom yang ada spasi
            Dim sql As String = "INSERT INTO siswa (nisn, nama, `NOMOR HP`, `NAMA WALI`, `NOMOR WALI`, alamat, id_guru) " &
                                "VALUES (@nisn, @nama, @no_hp, @nama_wali, @no_wali, @alamat, @id_guru)"

            Dim cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@nisn", TextBox1.Text)
            cmd.Parameters.AddWithValue("@nama", TextBox2.Text)
            cmd.Parameters.AddWithValue("@no_hp", TextBox3.Text)
            cmd.Parameters.AddWithValue("@nama_wali", TextBox4.Text)
            cmd.Parameters.AddWithValue("@no_wali", TextBox5.Text)
            cmd.Parameters.AddWithValue("@alamat", RichTextBox1.Text)
            cmd.Parameters.AddWithValue("@id_guru", CurrentUserId)  ' dari session login

            cmd.ExecuteNonQuery()
            MessageBox.Show("Data siswa berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.Close()
            Form1.tampilData()

        Catch ex As Exception
            MessageBox.Show("Gagal menyimpan data: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub
End Class
