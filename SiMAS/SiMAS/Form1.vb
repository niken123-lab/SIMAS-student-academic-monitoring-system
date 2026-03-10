Imports MySql.Data.MySqlClient
Imports System.Windows.Forms.DataVisualization.Charting

Public Class Form1
    Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Pastikan hanya tampil data kalau sudah login
        If CurrentUserId <> 0 Then
            tampilData()
        Else
            ' Kalau belum login, kembali ke form login
            MessageBox.Show("Silakan login terlebih dahulu!", "Akses ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Hide()
            Form6.Show()
        End If
    End Sub
    Public Sub tampilData()
        Label12.Text = CurrentUserNama
        Try
            conn.Open()

            ' === Jumlah siswa ===
            Dim cmdSiswa As New MySqlCommand("SELECT COUNT(*) FROM siswa", conn)
            Dim jumlahSiswa As Integer = Convert.ToInt32(cmdSiswa.ExecuteScalar())
            Label6.Text = jumlahSiswa.ToString()

            ' === Jumlah wali kelas ===
            Dim cmdGuru As New MySqlCommand("SELECT COUNT(*) FROM user", conn)
            Dim jumlahGuru As Integer = Convert.ToInt32(cmdGuru.ExecuteScalar())
            Label8.Text = jumlahGuru.ToString()

            ' === Jumlah kelas unik ===
            Dim cmdKelas As New MySqlCommand("SELECT COUNT(DISTINCT kelas) FROM user", conn)
            Dim jumlahKelas As Integer = Convert.ToInt32(cmdKelas.ExecuteScalar())
            Label2.Text = jumlahKelas.ToString()

            conn.Close()

            ' === Panggil fungsi untuk menampilkan grafik ===
            TampilkanGrafik(jumlahSiswa, jumlahGuru, jumlahKelas)

        Catch ex As Exception
            MessageBox.Show("Gagal menampilkan data: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
    Private Sub TampilkanGrafik(jumlahSiswa As Integer, jumlahGuru As Integer, jumlahKelas As Integer)
        Chart1.Series.Clear()
        Chart1.Titles.Clear()
        Chart1.ChartAreas.Clear()

        Dim area As New ChartArea()
        Chart1.ChartAreas.Add(area)

        ' Hilangkan garis kotak/grid
        With area
            .AxisX.MajorGrid.Enabled = False

            .AxisX.LineColor = Color.Transparent
            .AxisY.LineColor = Color.Transparent
            .AxisX.MajorTickMark.Enabled = False
            .AxisY.MajorTickMark.Enabled = False

        End With

        Dim series As New Series("Statistik")
        series.ChartType = SeriesChartType.Column
        series.Points.AddXY("Siswa", jumlahSiswa)
        series.Points.AddXY("Wali Kelas", jumlahGuru)
        series.Points.AddXY("Kelas", jumlahKelas)

        Chart1.Series.Add(series)


        Chart1.ChartAreas(0).AxisX.Title = "Kategori"
        Chart1.ChartAreas(0).AxisY.Title = "Jumlah"
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(230, 230, 250)

        Timer1.Interval = 1000
        Timer1.Start()

    End Sub

    Private Sub CenterHeader()
        If GroupBox3 Is Nothing Then Return

        ' Label1 = SIMAS, Label2 = Sistem Informasi Manajemen Siswa
        Dim centerX As Integer = GroupBox3.Left + (GroupBox3.Width - Label1.Width) \ 2
        Dim topY As Integer = GroupBox3.Top + 15 ' Jarak dari atas GroupBox

        Label1.Left = centerX
        Label1.Top = topY

        Label11.Left = GroupBox3.Left + (GroupBox3.Width - Label11.Width) \ 2
        Label11.Top = Label1.Bottom + 5
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form19.Show()
        Me.Hide()

    End Sub



    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form17.Show()
        Me.Hide()
    End Sub



    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ToolStripStatusLabel1.Text = "Date/Time: " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
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

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label13_Click(sender As Object, e As EventArgs) Handles Label13.Click

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub GroupBox3_Enter(sender As Object, e As EventArgs) Handles GroupBox3.Enter

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form22.Show()
        Me.Hide()
    End Sub

    Private Sub ToolStripStatusLabel1_Click(sender As Object, e As EventArgs) Handles ToolStripStatusLabel1.Click

    End Sub
End Class