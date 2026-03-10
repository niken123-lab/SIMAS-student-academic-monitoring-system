Imports MySql.Data.MySqlClient

Public Class Form8
    ' Koneksi
    Private ReadOnly connStr As String = "server=localhost;user id=root;password=;database=simas"

    ' Simpan NISN lama untuk WHERE saat update
    Public Property OldNISN As String = ""

    Public Sub New()
        InitializeComponent()
    End Sub

    ' Tombol CHANGE
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' Validasi sederhana
            If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox2.Text) Then
                MessageBox.Show("NISN dan NAMA wajib diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Using conn As New MySqlConnection(connStr)
                conn.Open()

                ' Pakai backtick untuk kolom yang ada spasi
                Dim sql As String =
                    "UPDATE `siswa` SET " &
                    " `NISN`=@nisn, " &
                    " `NAMA`=@nama, " &
                    " `NOMOR HP`=@no_hp, " &
                    " `NAMA WALI`=@nama_wali, " &
                    " `NOMOR WALI`=@no_wali, " &
                    " `ALAMAT`=@alamat " &
                    "WHERE `NISN`=@old_nisn;"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@nisn", TextBox1.Text.Trim())          ' boleh diubah
                    cmd.Parameters.AddWithValue("@nama", TextBox2.Text.Trim())
                    cmd.Parameters.AddWithValue("@no_hp", TextBox3.Text.Trim())
                    cmd.Parameters.AddWithValue("@nama_wali", TextBox4.Text.Trim())
                    cmd.Parameters.AddWithValue("@no_wali", TextBox5.Text.Trim())
                    cmd.Parameters.AddWithValue("@alamat", RichTextBox1.Text.Trim())
                    cmd.Parameters.AddWithValue("@old_nisn", If(String.IsNullOrEmpty(OldNISN), TextBox1.Text.Trim(), OldNISN))

                    Dim rows = cmd.ExecuteNonQuery()
                    If rows > 0 Then
                        MessageBox.Show("Data siswa berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Me.Close()
                    Else
                        MessageBox.Show("Tidak ada baris yang terubah. Periksa NISN.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Gagal memperbarui data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
