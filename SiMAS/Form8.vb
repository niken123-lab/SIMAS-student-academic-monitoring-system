Imports MySql.Data.MySqlClient

Public Class Form8
    Public Event DataUpdated(updateData As String())
    Private nisnAwal As String

    Public Sub New(data1 As String, data2 As String, data3 As String, data4 As String, data5 As String, data6 As String)
        InitializeComponent()
        nisnAwal = data1  ' simpan NISN lama untuk WHERE

        TextBox1.Text = data1
        TextBox2.Text = data2
        TextBox3.Text = data3
        TextBox4.Text = data4
        TextBox5.Text = data5
        RichTextBox1.Text = data6
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' ✅ gunakan backtick untuk kolom dengan spasi
        Dim sql As String = "UPDATE siswa SET " &
                            "nisn=@nisn, " &
                            "nama=@nama, " &
                            "`NOMOR HP`=@no_hp, " &
                            "`NAMA WALI`=@nama_wali, " &
                            "`NOMOR WALI`=@no_wali, " &
                            "alamat=@alamat " &
                            "WHERE nisn=@nisnAwal"

        Try
            Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas"),
                  cmd As New MySqlCommand(sql, conn)

                cmd.Parameters.AddWithValue("@nisn", TextBox1.Text)
                cmd.Parameters.AddWithValue("@nama", TextBox2.Text)
                cmd.Parameters.AddWithValue("@no_hp", TextBox3.Text)
                cmd.Parameters.AddWithValue("@nama_wali", TextBox4.Text)
                cmd.Parameters.AddWithValue("@no_wali", TextBox5.Text)
                cmd.Parameters.AddWithValue("@alamat", RichTextBox1.Text)
                cmd.Parameters.AddWithValue("@nisnAwal", nisnAwal)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Data berhasil diupdate!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Refresh DataGridView di Form1
            DirectCast(Application.OpenForms("Form1"), Form1).tampilData()

            ' Sinkronisasi row (opsional)
            RaiseEvent DataUpdated(New String() {
                TextBox1.Text,
                TextBox2.Text,
                TextBox3.Text,
                TextBox4.Text,
                TextBox5.Text,
                RichTextBox1.Text
            })

            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Gagal update: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
