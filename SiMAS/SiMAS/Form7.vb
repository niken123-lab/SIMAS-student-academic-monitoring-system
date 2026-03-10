Imports MySql.Data.MySqlClient

Public Class Form7
    Private connStr As String = "server=localhost;user id=root;password=;database=simas"
    Private _kelasDipilih As String

    ' Constructor menerima nama kelas dari Form22
    Public Sub New(kelas As String)
        InitializeComponent()
        _kelasDipilih = kelas
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' Validasi input
            If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse
               String.IsNullOrWhiteSpace(TextBox2.Text) Then
                MessageBox.Show("NISN dan NAMA wajib diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Ambil id_guru berdasarkan kelas
            Dim idGuru As Integer = -1
            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Dim cmd As New MySqlCommand("SELECT id_guru FROM user WHERE kelas=@kelas", conn)
                cmd.Parameters.AddWithValue("@kelas", _kelasDipilih)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    idGuru = Convert.ToInt32(result)
                Else
                    MessageBox.Show("Kelas tidak ditemukan di tabel user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
            End Using

            ' Simpan data siswa baru
            Using conn As New MySqlConnection(connStr)
                conn.Open()

                Dim sql As String = "
                    INSERT INTO `siswa` 
                        (`NISN`, `NAMA`, `NOMOR HP`, `NAMA WALI`, `NOMOR WALI`, `ALAMAT`, `id_guru`) 
                    VALUES 
                        (@nisn, @nama, @no_hp, @nama_wali, @no_wali, @alamat, @id_guru)
                "

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@nisn", TextBox1.Text.Trim())
                    cmd.Parameters.AddWithValue("@nama", TextBox2.Text.Trim())
                    cmd.Parameters.AddWithValue("@no_hp", TextBox3.Text.Trim())
                    cmd.Parameters.AddWithValue("@nama_wali", TextBox4.Text.Trim())
                    cmd.Parameters.AddWithValue("@no_wali", TextBox5.Text.Trim())
                    cmd.Parameters.AddWithValue("@alamat", RichTextBox1.Text.Trim())
                    cmd.Parameters.AddWithValue("@id_guru", idGuru)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show($"✅ Data siswa berhasil disimpan untuk kelas {_kelasDipilih}!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("❌ Gagal menyimpan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
