Imports MySql.Data.MySqlClient

Public Class Form14

    Public Sub RefreshData()
        tampilNilai()
    End Sub

    Private Sub Form14_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadMataPelajaran()
        LoadSemester()
        tampilNilai()
    End Sub
    Private Sub LoadMataPelajaran()
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()
            Dim sql As String = "SELECT id_mapel, nama_mapel FROM mata_pelajaran"
            Dim cmd As New MySqlCommand(sql, conn)
            Dim dt As New DataTable()
            dt.Load(cmd.ExecuteReader())
            ComboBox1.DataSource = dt
            ComboBox1.DisplayMember = "nama_mapel"
            ComboBox1.ValueMember = "id_mapel"
            ComboBox1.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Gagal memuat mata pelajaran: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub LoadSemester()
        ComboBox2.Items.Clear()
        ComboBox2.Items.Add("1")
        ComboBox2.Items.Add("2")
    End Sub


    '================= TAMPILKAN NILAI =================
    Public Sub tampilNilai()
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            ' ambil semua siswa milik guru login
            Dim sql As String = "
            SELECT 
                s.NISN,
                s.NAMA,
                n.nilai_uh1 AS UH1,
                n.nilai_uh2 AS UH2,
                n.nilai_uh3 AS UH3,
                n.nilai_uh4 AS UH4,
                n.nilai_uh5 AS UH5,
                n.nilai_uts AS UTS,
                n.nilai_sikap AS Sikap,
                n.rata_rata AS `Rata-rata`
            FROM siswa s
            LEFT JOIN nilai n 
                ON s.NISN = n.NISN 
                AND s.id_guru = n.id_guru
                AND (@id_mapel IS NULL OR n.id_mapel = @id_mapel)
                AND (@semester IS NULL OR n.semester = @semester)
            WHERE s.id_guru = @id_guru
            ORDER BY s.NAMA
        "

            Dim da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@id_guru", CurrentUserId)

            ' handle null parameter agar tidak error
            Dim mapelParam As Object = If(ComboBox1.SelectedValue Is Nothing, DBNull.Value, ComboBox1.SelectedValue)
            Dim semParam As Object = If(String.IsNullOrEmpty(ComboBox2.Text), DBNull.Value, ComboBox2.Text)

            da.SelectCommand.Parameters.AddWithValue("@id_mapel", mapelParam)
            da.SelectCommand.Parameters.AddWithValue("@semester", semParam)

            Dim ds As New DataSet()
            da.Fill(ds, "nilai")
            DataGridView1.DataSource = ds.Tables("nilai")

        Catch ex As Exception
            MessageBox.Show("Gagal mengambil data nilai: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub



    '================= INPUT NILAI BARU =================
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih siswa terlebih dahulu untuk input nilai")
            Exit Sub
        End If

        Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim nisn As String = row.Cells("NISN").Value.ToString()
        Dim nama As String = row.Cells("Nama").Value.ToString()

        ' Kirim ke Form9 (input nilai baru)
        If ComboBox1.SelectedValue Is Nothing OrElse String.IsNullOrEmpty(ComboBox2.Text) Then
            MessageBox.Show("Pilih mata pelajaran dan semester terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim idMapel As Integer = CInt(ComboBox1.SelectedValue)
        Dim semester As String = ComboBox2.Text
        Dim frm As New Form9(nisn, nama, idMapel, semester)
        frm.ShowDialog()
        tampilNilai()


    End Sub

    '================= EDIT NILAI =================
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih siswa yang mau diubah nilainya!")
            Exit Sub
        End If

        If ComboBox1.SelectedValue Is Nothing OrElse String.IsNullOrEmpty(ComboBox2.Text) Then
            MessageBox.Show("Pilih mata pelajaran dan semester terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim idMapel As Integer = CInt(ComboBox1.SelectedValue)
        Dim semester As String = ComboBox2.Text

        Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim nisn As String = row.Cells("NISN").Value.ToString()
        Dim nama As String = row.Cells("Nama").Value.ToString()

        ' Ambil nilai lama
        Dim uh1 As String = row.Cells("UH1").Value.ToString()
        Dim uh2 As String = row.Cells("UH2").Value.ToString()
        Dim uh3 As String = row.Cells("UH3").Value.ToString()
        Dim uh4 As String = row.Cells("UH4").Value.ToString()
        Dim uh5 As String = row.Cells("UH5").Value.ToString()
        Dim uts As String = row.Cells("UTS").Value.ToString()
        Dim sikap As String = row.Cells("Sikap").Value.ToString()

        Dim frm As New Form10(nisn, nama, uh1, uh2, uh3, uh4, uh5, uts, sikap, idMapel, semester)
        frm.ShowDialog()
        tampilNilai()
    End Sub

    '================= HAPUS NILAI =================
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih siswa yang mau dihapus nilainya!")
            Exit Sub
        End If

        ' ambil nisn
        Dim nisn As String = DataGridView1.SelectedRows(0).Cells("NISN").Value.ToString()

        ' pastikan mapel & semester dipilih
        If ComboBox1.SelectedValue Is Nothing OrElse String.IsNullOrEmpty(ComboBox2.Text) Then
            MessageBox.Show("Pilih mata pelajaran dan semester terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' konfirmasi hapus
        If MessageBox.Show($"Yakin ingin menghapus nilai siswa {nisn} pada mapel dan semester yang dipilih?",
                       "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then Exit Sub

        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            ' hanya hapus nilai sesuai guru, mapel, dan semester aktif
            Dim sql As String = "
            DELETE FROM nilai 
            WHERE NISN=@nisn 
              AND id_guru=@id_guru 
              AND id_mapel=@id_mapel 
              AND semester=@semester
        "

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@nisn", nisn)
                cmd.Parameters.AddWithValue("@id_guru", CurrentUserId)
                cmd.Parameters.AddWithValue("@id_mapel", ComboBox1.SelectedValue)
                cmd.Parameters.AddWithValue("@semester", ComboBox2.Text)
                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Data nilai berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            tampilNilai()

        Catch ex As Exception
            MessageBox.Show("Error hapus nilai: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub


    '================= NAVIGASI =================
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Form1.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form16.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form11.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form3.Show()
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearSession()
            Form6.TextBox1.Text = ""
            Form6.TextBox2.Text = ""
            Form6.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub Label11_Click(sender As Object, e As EventArgs) Handles Label11.Click

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        tampilNilai()
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        tampilNilai()
    End Sub
End Class
