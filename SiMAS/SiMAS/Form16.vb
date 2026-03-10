Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports MySql.Data.MySqlClient

Public Class Form16

    Private Sub Form16_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSemester()
    End Sub

    Private Sub LoadSemester()
        ComboBox2.Items.Clear()
        ComboBox2.Items.Add("1")
        ComboBox2.Items.Add("2")
        ComboBox2.SelectedIndex = 0
    End Sub

    ' ==================== TAMPILKAN REKAP NILAI ====================
    Private Sub TampilRekap()
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            Dim sql As String = "
    SELECT 
        s.NISN,
        s.NAMA,
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Pendidikan Pancasila dan Kewarganegaraan (PPKn)' THEN n.rata_rata END), 2) AS 'PPKn',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Bahasa Indonesia' THEN n.rata_rata END), 2) AS 'B. Indonesia',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Matematika' THEN n.rata_rata END), 2) AS 'Matematika',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Ilmu Pengetahuan Alam dan Sosial (IPAS)' THEN n.rata_rata END), 2) AS 'IPAS',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Seni Budaya dan Keterampilan (SBK)' THEN n.rata_rata END), 2) AS 'SBK',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Pendidikan Jasmani, Olahraga, dan Kesehatan (PJOK)' THEN n.rata_rata END), 2) AS 'PJOK',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Pendidikan Agama dan Budi Pekerti' THEN n.rata_rata END), 2) AS 'Agama',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Bahasa Inggris' THEN n.rata_rata END), 2) AS 'B. Inggris',
        ROUND(AVG(CASE WHEN m.nama_mapel = 'Bahasa Jawa' THEN n.rata_rata END), 2) AS 'B. Jawa',
        ROUND(AVG(n.rata_rata), 2) AS 'Rata-rata Total'
    FROM siswa s
    LEFT JOIN nilai n 
        ON s.NISN = n.NISN 
        AND s.id_guru = n.id_guru 
        AND n.semester = @semester
    LEFT JOIN mata_pelajaran m 
        ON n.id_mapel = m.id_mapel
    WHERE s.id_guru = @id_guru
    GROUP BY s.NISN, s.NAMA
    ORDER BY s.NAMA;
"


            Dim da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@id_guru", CurrentUserId)
            da.SelectCommand.Parameters.AddWithValue("@semester", ComboBox2.Text)

            Dim ds As New DataSet()
            da.Fill(ds, "rekap")
            DataGridView1.DataSource = ds.Tables("rekap")

            ' === HITUNG STATISTIK UNTUK LABEL ===
            Dim dt As DataTable = ds.Tables("rekap")

            If dt.Rows.Count > 0 Then
                ' Jumlah siswa
                Label2.Text = dt.Rows.Count.ToString()

                ' Ambil nilai rata-rata total untuk semua siswa
                Dim totalNilai As Decimal = 0
                Dim countNilai As Integer = 0
                Dim nilaiTertinggi As Decimal = Decimal.MinValue
                Dim nilaiTerendah As Decimal = Decimal.MaxValue

                For Each row As DataRow In dt.Rows
                    If Not IsDBNull(row("Rata-rata Total")) Then
                        Dim nilai As Decimal = Convert.ToDecimal(row("Rata-rata Total"))
                        totalNilai += nilai
                        countNilai += 1
                        If nilai > nilaiTertinggi Then nilaiTertinggi = nilai
                        If nilai < nilaiTerendah Then nilaiTerendah = nilai
                    End If
                Next

                If countNilai > 0 Then
                    Dim rataKelas As Decimal = totalNilai / countNilai
                    Label7.Text = Math.Round(rataKelas, 2).ToString()
                    Label9.Text = nilaiTertinggi.ToString()
                    Label11.Text = nilaiTerendah.ToString()
                Else
                    Label7.Text = "-"
                    Label9.Text = "-"
                    Label11.Text = "-"
                End If
            Else
                Label2.Text = "0"
                Label7.Text = "-"
                Label9.Text = "-"
                Label11.Text = "-"
            End If


        Catch ex As Exception
            MessageBox.Show("Gagal mengambil data rekap: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' Saat semester diganti
    Private Sub ComboBoxSemester_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        TampilRekap()
    End Sub

    ' ==================== EXPORT PDF ====================

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Try
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Pilih satu siswa terlebih dahulu dari tabel.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim nisn As String = row.Cells("NISN").Value.ToString()
            Dim nama As String = row.Cells("NAMA").Value.ToString()

            Dim sfd As New SaveFileDialog()
            sfd.Filter = "PDF Files (*.pdf)|*.pdf"
            sfd.FileName = "Raport_" & nama & "_Semester" & ComboBox2.Text & ".pdf"

            If sfd.ShowDialog() = DialogResult.OK Then
                Dim path As String = sfd.FileName
                Dim doc As New Document(PageSize.A4, 40, 40, 40, 40)
                PdfWriter.GetInstance(doc, New FileStream(path, FileMode.Create))
                doc.Open()

                ' === HEADER ===
                doc.Add(New Paragraph("DINAS PENDIDIKAN KABUPATEN SIDOARJO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)) With {.Alignment = Element.ALIGN_CENTER})
                doc.Add(New Paragraph("LAPORAN HASIL BELAJAR SISWA", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)) With {.Alignment = Element.ALIGN_CENTER})
                doc.Add(New Paragraph("SD NEGERI GANTING", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)) With {.Alignment = Element.ALIGN_CENTER})
                doc.Add(New Paragraph(" "))

                ' === IDENTITAS SISWA ===
                Dim fIsi As Font = FontFactory.GetFont(FontFactory.HELVETICA, 11)
                doc.Add(New Paragraph("Nama Siswa     : " & nama, fIsi))
                doc.Add(New Paragraph("NISN               : " & nisn, fIsi))
                doc.Add(New Paragraph("Kelas              : " & CurrentUserKelas, fIsi))
                doc.Add(New Paragraph("Semester        : " & ComboBox2.Text, fIsi))
                doc.Add(New Paragraph("Tahun Pelajaran : 2025/2026", fIsi))
                doc.Add(New Paragraph(" "))

                ' === REKAP NILAI AKADEMIK ===
                doc.Add(New Paragraph("Rekap Nilai Akademik", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                doc.Add(New Paragraph(" "))

                Dim pdfTable As New PdfPTable(DataGridView1.Columns.Count - 2) ' tanpa NISN & Nama
                pdfTable.WidthPercentage = 100

                ' Header kolom
                For i As Integer = 2 To DataGridView1.Columns.Count - 1
                    Dim colHeader As String = DataGridView1.Columns(i).HeaderText
                    Dim cell As New PdfPCell(New Phrase(colHeader, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY
                    cell.HorizontalAlignment = Element.ALIGN_CENTER
                    pdfTable.AddCell(cell)
                Next

                ' Isi nilai
                For i As Integer = 2 To DataGridView1.Columns.Count - 1
                    Dim val As String = If(row.Cells(i).Value Is Nothing, "-", row.Cells(i).Value.ToString())
                    Dim cell As New PdfPCell(New Phrase(val, FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                    cell.HorizontalAlignment = Element.ALIGN_CENTER
                    pdfTable.AddCell(cell)
                Next

                doc.Add(pdfTable)
                doc.Add(New Paragraph(" "))

                ' === CATATAN PRESTASI ===
                Dim rataSiswa As Decimal = 0
                Decimal.TryParse(row.Cells("Rata-rata Total").Value.ToString(), rataSiswa)

                ' Hitung rata-rata kelas
                Dim totalKelas As Decimal = 0
                Dim jumlahKelas As Integer = 0
                For Each r As DataGridViewRow In DataGridView1.Rows
                    If r.Cells("Rata-rata Total").Value IsNot Nothing AndAlso Not DBNull.Value.Equals(r.Cells("Rata-rata Total").Value) Then
                        Dim val As Decimal
                        If Decimal.TryParse(r.Cells("Rata-rata Total").Value.ToString(), val) Then
                            totalKelas += val
                            jumlahKelas += 1
                        End If
                    End If
                Next

                Dim rataKelas As Decimal = If(jumlahKelas > 0, totalKelas / jumlahKelas, 0)
                Dim catatan As String
                If rataSiswa >= rataKelas Then
                    catatan = "Nilai rata-rata kamu sudah baik. Pertahankan prestasimu!"
                Else
                    catatan = "Nilai rata-rata kamu masih di bawah rata-rata kelas. Tingkatkan semangat belajar agar hasil lebih baik di semester berikutnya!"
                End If

                doc.Add(New Paragraph("Catatan Wali Kelas:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                doc.Add(New Paragraph(catatan, FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                doc.Add(New Paragraph(" "))

                ' === REKAP ABSENSI ===
                doc.Add(New Paragraph("Rekap Absensi", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                doc.Add(New Paragraph(" "))

                Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                    conn.Open()
                    Dim sqlAbsensi As String = "
                    SELECT sakit, izin, alpha, 
                           (60 - (sakit + izin + alpha)) AS total_hadir
                    FROM rekap_absensi
                    WHERE nisn=@nisn
                "
                    Dim cmd As New MySqlCommand(sqlAbsensi, conn)
                    cmd.Parameters.AddWithValue("@nisn", nisn)

                    Dim rd As MySqlDataReader = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim absensiTable As New PdfPTable(4)
                        absensiTable.WidthPercentage = 100

                        Dim headersAbs() As String = {"Sakit", "Izin", "Alpha", "Total Hadir"}
                        For Each h As String In headersAbs
                            Dim cell As New PdfPCell(New Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY
                            cell.HorizontalAlignment = Element.ALIGN_CENTER
                            absensiTable.AddCell(cell)
                        Next

                        absensiTable.AddCell(rd("sakit").ToString())
                        absensiTable.AddCell(rd("izin").ToString())
                        absensiTable.AddCell(rd("alpha").ToString())
                        absensiTable.AddCell(rd("total_hadir").ToString())

                        doc.Add(absensiTable)
                    Else
                        doc.Add(New Paragraph("Data absensi belum tersedia untuk siswa ini.", FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                    End If
                    rd.Close()
                End Using

                doc.Add(New Paragraph(" "))
                doc.Add(New Paragraph("Pertahankan prestasimu yang telah dicapai dengan terus meningkatkan kedisiplinan, tanggung jawab, dan semangat belajar agar hasil yang lebih baik dapat diraih di masa mendatang.", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10)))
                doc.Add(New Paragraph(" "))

                ' === TANDA TANGAN ===
                doc.Add(New Paragraph("Sidoarjo, " & DateTime.Now.ToString("dd MMMM yyyy")))
                doc.Add(New Paragraph("Wali Kelas,", FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                doc.Add(New Paragraph(" "))
                doc.Add(New Paragraph(" "))
                doc.Add(New Paragraph(CurrentUserNama, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))

                doc.Close()

                MessageBox.Show("Raport siswa berhasil diekspor: " & path, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Process.Start(path)
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal ekspor PDF: " & ex.Message)
        End Try
    End Sub


    ' Navigasi antar form
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form15.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form14.Show()
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

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearSession()
            Form6.TextBox1.Text = ""
            Form6.TextBox2.Text = ""
            Form6.Show()
            Me.Hide()
        End If
    End Sub
End Class
