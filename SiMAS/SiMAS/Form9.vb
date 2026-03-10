Imports MySql.Data.MySqlClient
Imports System.Globalization

Public Class Form9
    Private _nisn As String
    Private _nama As String
    Private _idMapel As Integer
    Private _semester As String

    Public Sub New(nisn As String, nama As String, idMapel As Integer, semester As String)
        InitializeComponent()
        _nisn = nisn
        _nama = nama
        _idMapel = idMapel
        _semester = semester
    End Sub

    ' =========================
    ' Fungsi bantu parsing angka
    ' =========================
    Private Function ToNullableDecimal(tb As TextBox) As Object
        Dim cleaned As String = tb.Text.Trim().Replace(",", ".")
        Dim d As Decimal
        ' Coba parse pakai format global (bisa 90,5 atau 90.5)
        If Decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, d) Then
            Return d
        End If
        Return DBNull.Value
    End Function

    ' =========================
    ' Hitung rata-rata otomatis
    ' =========================
    Private Function HitungRataRata(values As List(Of Decimal?)) As Object
        Dim angka = values.Where(Function(v) v.HasValue).Select(Function(v) v.Value).ToList()
        If angka.Count = 0 Then Return DBNull.Value
        Return Math.Round(angka.Average(), 2)
    End Function

    ' =========================
    ' Klik tombol SIMPAN
    ' =========================
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' --- Ambil nilai dari textbox ---
            Dim uh1 As Object = ToNullableDecimal(TextBox1)
            Dim uh2 As Object = ToNullableDecimal(TextBox2)
            Dim uh3 As Object = ToNullableDecimal(TextBox3)
            Dim uh4 As Object = ToNullableDecimal(TextBox4)
            Dim uh5 As Object = ToNullableDecimal(TextBox5)
            Dim uts As Object = ToNullableDecimal(TextBox6)
            Dim sikap As Object = ToNullableDecimal(TextBox7)

            ' --- Hitung rata-rata otomatis ---
            Dim rata As Object = HitungRataRata(New List(Of Decimal?) From {
                If(TypeOf uh1 Is Decimal, CType(uh1, Decimal), CType(Nothing, Decimal?)),
                If(TypeOf uh2 Is Decimal, CType(uh2, Decimal), CType(Nothing, Decimal?)),
                If(TypeOf uh3 Is Decimal, CType(uh3, Decimal), CType(Nothing, Decimal?)),
                If(TypeOf uh4 Is Decimal, CType(uh4, Decimal), CType(Nothing, Decimal?)),
                If(TypeOf uh5 Is Decimal, CType(uh5, Decimal), CType(Nothing, Decimal?)),
                If(TypeOf uts Is Decimal, CType(uts, Decimal), CType(Nothing, Decimal?)),
                If(TypeOf sikap Is Decimal, CType(sikap, Decimal), CType(Nothing, Decimal?))
            })

            ' --- Debug log (sementara) ---
            'MessageBox.Show($"UH1={uh1}, UH2={uh2}, UTS={uts}, Sikap={sikap}, Rata={rata}")

            Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                conn.Open()

                ' --- Gunakan query ON DUPLICATE agar lebih simpel ---
                Dim sql As String = "
                    INSERT INTO nilai 
                        (NISN, NAMA, id_guru, id_mapel, semester,
                         nilai_uh1, nilai_uh2, nilai_uh3, nilai_uh4, nilai_uh5,
                         nilai_uts, nilai_sikap, rata_rata)
                    VALUES
                        (@nisn, @nama, @id_guru, @id_mapel, @semester,
                         @uh1, @uh2, @uh3, @uh4, @uh5, @uts, @sikap, @rata)
                    ON DUPLICATE KEY UPDATE
                        nilai_uh1=@uh1,
                        nilai_uh2=@uh2,
                        nilai_uh3=@uh3,
                        nilai_uh4=@uh4,
                        nilai_uh5=@uh5,
                        nilai_uts=@uts,
                        nilai_sikap=@sikap,
                        rata_rata=@rata;
                "

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@nisn", _nisn)
                    cmd.Parameters.AddWithValue("@nama", _nama)
                    cmd.Parameters.AddWithValue("@id_guru", CurrentUserId)
                    cmd.Parameters.AddWithValue("@id_mapel", _idMapel)
                    cmd.Parameters.AddWithValue("@semester", _semester)
                    cmd.Parameters.AddWithValue("@uh1", uh1)
                    cmd.Parameters.AddWithValue("@uh2", uh2)
                    cmd.Parameters.AddWithValue("@uh3", uh3)
                    cmd.Parameters.AddWithValue("@uh4", uh4)
                    cmd.Parameters.AddWithValue("@uh5", uh5)
                    cmd.Parameters.AddWithValue("@uts", uts)
                    cmd.Parameters.AddWithValue("@sikap", sikap)
                    cmd.Parameters.AddWithValue("@rata", rata)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("✅ Nilai berhasil disimpan ke database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("❌ Error input nilai: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
