Imports MySql.Data.MySqlClient

Public Class Form10
    Private _nisn As String
    Private _idMapel As Integer
    Private _semester As String

    ' === Konstruktor baru ===
    Public Sub New(nisn As String, nama As String, uh1 As String, uh2 As String, uh3 As String, uh4 As String,
                   uh5 As String, uts As String, sikap As String, idMapel As Integer, semester As String)
        InitializeComponent()

        _nisn = nisn
        _idMapel = idMapel
        _semester = semester

        ' isi textbox
        TextBox1.Text = uh1
        TextBox2.Text = uh2
        TextBox3.Text = uh3
        TextBox4.Text = uh4
        TextBox5.Text = uh5
        TextBox6.Text = uts
        TextBox7.Text = sikap
    End Sub

    ' ============== UPDATE NILAI SISWA ==============
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' ambil nilai dari textbox
            Dim uh1 As Decimal = Val(TextBox1.Text)
            Dim uh2 As Decimal = Val(TextBox2.Text)
            Dim uh3 As Decimal = Val(TextBox3.Text)
            Dim uh4 As Decimal = Val(TextBox4.Text)
            Dim uh5 As Decimal = Val(TextBox5.Text)
            Dim uts As Decimal = Val(TextBox6.Text)
            Dim sikap As Decimal = Val(TextBox7.Text)

            ' hitung rata-rata
            Dim rata As Decimal = Math.Round((uh1 + uh2 + uh3 + uh4 + uh5 + uts + sikap) / 7, 2)

            Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                conn.Open()

                Dim sql As String = "
                    UPDATE nilai SET
                        nilai_uh1=@uh1,
                        nilai_uh2=@uh2,
                        nilai_uh3=@uh3,
                        nilai_uh4=@uh4,
                        nilai_uh5=@uh5,
                        nilai_uts=@uts,
                        nilai_sikap=@sikap,
                        rata_rata=@rata
                    WHERE NISN=@nisn AND id_guru=@id_guru
                          AND id_mapel=@id_mapel AND semester=@semester
                "

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@uh1", uh1)
                    cmd.Parameters.AddWithValue("@uh2", uh2)
                    cmd.Parameters.AddWithValue("@uh3", uh3)
                    cmd.Parameters.AddWithValue("@uh4", uh4)
                    cmd.Parameters.AddWithValue("@uh5", uh5)
                    cmd.Parameters.AddWithValue("@uts", uts)
                    cmd.Parameters.AddWithValue("@sikap", sikap)
                    cmd.Parameters.AddWithValue("@rata", rata)
                    cmd.Parameters.AddWithValue("@nisn", _nisn)
                    cmd.Parameters.AddWithValue("@id_guru", CurrentUserId)
                    cmd.Parameters.AddWithValue("@id_mapel", _idMapel)
                    cmd.Parameters.AddWithValue("@semester", _semester)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("✅ Nilai siswa berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("❌ Terjadi kesalahan saat mengubah nilai: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
