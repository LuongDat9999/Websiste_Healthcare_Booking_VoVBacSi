using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DoAnCNPM.Models;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace DAPMBSVOV.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            DataModel db = new DataModel();
            return View(); // Trả về view 'Index.cshtml'
        }

        // Bài viết
        public IActionResult DMBaiViet()
        {
            DataModel db = new DataModel();
            ViewBag.listLBV = db.get("SELECT * from LOAIBAIVIET");
            ViewBag.listBV = db.get("SELECT * from BAIVIET");
            ViewBag.listKB = db.get("SELECT * from KHOABENH");
            return View(); 
        }
        [HttpPost]
        public IActionResult ThemBaiViet(string tieudebv, 
                                        string noidungbv, 
                                        string makb, 
                                        string linkytb,  
                                        string malbv, 
                                        string luotxem)
        {
            DateTime ngaydang = DateTime.Now;
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC AddBAIVIET N'" +tieudebv+ "', N'" +noidungbv+ "'," +makb+ ",'" +linkytb+ "','" +ngaydang+ "'," +malbv+ "," +luotxem+ ";");
            return RedirectToAction("DMBaiViet", "Admin"); 
        }

        [HttpPost]
        public IActionResult XoaBaiViet(string id)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC DeleteBAIVIET " + id + ";");
            return RedirectToAction("DMBaiViet", "Admin"); 
        }

        public IActionResult TimBaiViet(string mabv)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC FindBAIVIET_ID " +mabv+ ";");            
            ViewBag.listLBV = db.get("SELECT * from LOAIBAIVIET");
            ViewBag.listKB = db.get("SELECT * from KHOABENH");
            return View(); 
        }

        public IActionResult ChiTietBaiViet(string mabv)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC FindBAIVIET_ID " +mabv+ ";");      
            ViewBag.listLBV = db.get("SELECT * from LOAIBAIVIET");
            ViewBag.listKB = db.get("SELECT * from KHOABENH");      
            return View(); 
        }

        [HttpPost]
        public IActionResult SuaBaiViet(string mabv,
                                        string tieudebv, 
                                        string noidungbv, 
                                        string makb, 
                                        string linkytb,  
                                        string malbv, 
                                        string luotxem)
        {
            DateTime ngaydang = DateTime.Now;
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC UpdateBAIVIET " +mabv+ ", N'" +tieudebv+ "', N'" +noidungbv+ "'," +makb+ ",'" +linkytb+ "','" +ngaydang+ "'," +malbv+ "," +luotxem+ ";");
            return RedirectToAction("DMBaiViet", "Admin"); 
        }
        // End Bài viết

        // Chuyên ngành
        public IActionResult DMChuyenNganh()
        {
            DataModel db = new DataModel();
            ViewBag.listCN = db.get("SELECT * from CHUYENNGANH");
            return View(); 
        }

        [HttpPost]
        public IActionResult ThemChuyenNganh(string tencn, IFormFile hinhanh)
        {
            if (hinhanh == null || hinhanh.Length == 0)
            {
                TempData["Error"] = "Vui lòng tải lên hình ảnh hợp lệ!";
                return RedirectToAction("DMChuyenNganh", "Admin");
            }

            try
            {
                DataModel db = new DataModel();

                // Lấy tên tệp
                string namefile = Path.GetFileName(hinhanh.FileName);

                // Đường dẫn để lưu tệp
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                string filePath = Path.Combine(uploadsFolder, namefile);

                // Lưu tệp
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    hinhanh.CopyTo(stream);
                }
            db.get($"EXEC sp_ThemChuyenNganh N'{tencn}','{namefile}';");
            TempData["Message"] = "Thêm chuyên ngành thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction("DMChuyenNganh", "Admin");
        }

        public IActionResult TimChuyenNganh(string macn) 
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC sp_TimChuyenNganhTheoMa " + macn + ";");
            return View();
        }

        [HttpPost]
        public IActionResult SuaChuyenNganh(string macn, string tencn, IFormFile hinhanh)
        {
            if (hinhanh == null || hinhanh.Length == 0)
            {
                TempData["Error"] = "Vui lòng tải lên hình ảnh hợp lệ!";
                return RedirectToAction("DMChuyenNganh", "Admin");
            }

            try
            {
                DataModel db = new DataModel();

                // Lấy tên tệp
                string namefile = Path.GetFileName(hinhanh.FileName);

                // Đường dẫn để lưu tệp
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                string filePath = Path.Combine(uploadsFolder, namefile);

                // Lưu tệp
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    hinhanh.CopyTo(stream);
                }
            db.get($"EXEC sp_SuaChuyenNganh {macn}, N'{tencn}', N'{namefile}';");
            TempData["Message"] = "Sửa chuyên ngành thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction("DMChuyenNganh", "Admin");
        }

        [HttpPost]
        public IActionResult XoaChuyenNganh(string macn) 
        {
            DataModel db = new DataModel();
            try
            {
                // Gọi stored procedure để xóa chuyên ngành
                db.get("EXEC sp_XoaChuyenNganh " + macn + ";");
                TempData["Message"] = "Xóa chuyên ngành thành công!";
            }
            catch (Exception)
            {
                // Nếu xảy ra lỗi, thông báo lỗi liên quan đến khóa ngoại
                TempData["Error"] = "Chuyên ngành được chọn có liên kết với bác sĩ. Không xóa thành công !!!!";
            }
            return RedirectToAction("DMChuyenNganh", "Admin");
        }
        // End Chuyên ngành

        // Khoa bệnh
        public IActionResult DMKhoaBenh()
        {
            DataModel db = new DataModel();
            ViewBag.listKB = db.get("SELECT * from KHOABENH");
            return View(); 
        }

        [HttpPost]
        public IActionResult ThemKhoaBenh(string tenkb, string mota)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC sp_ThemKhoaBenh N'" + tenkb + "', N'" + mota + "';");
            return RedirectToAction("DMKhoaBenh", "Admin");
        }

        [HttpPost]
        public IActionResult XoaKhoaBenh(string id)
        {
            DataModel db = new DataModel();
            try
            {
                // Gọi stored procedure để xóa khoa bệnh
                db.get("EXEC sp_XoaKhoaBenh " + id + ";");
                TempData["Message"] = "Xóa khoa bệnh thành công!";
            }
            catch (Exception)
            {
                // Nếu xảy ra lỗi, thông báo lỗi liên quan đến khóa ngoại
                TempData["Error"] = "Khoa bệnh được chọn có liên kết dữ liệu. Không xóa thành công !!!!";
            }
            return RedirectToAction("DMKhoaBenh", "Admin");
        }

        public IActionResult TimKhoaBenh(string makb) 
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC sp_TimKhoaBenhTheoMa " + makb + ";");
            return View();
        }

        [HttpPost]
        public IActionResult SuaKhoaBenh(string makb, string tenkb, string mota)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC sp_SuaKhoaBenh " + makb + ",N'" + tenkb + "', N'" + mota + "';");
            return RedirectToAction("DMKhoaBenh", "Admin");
        }
        // End Khoa bệnh

        // Bệnh viện
        public IActionResult DMBenhVien()
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("SELECT * from BENHVIEN");
            return View();
        }

        [HttpPost] 
        public IActionResult ThemBenhVien(string tenbv, IFormFile hinhanh)
        {
            if (hinhanh == null || hinhanh.Length == 0)
            {
                TempData["Error"] = "Vui lòng tải lên hình ảnh hợp lệ!";
                return RedirectToAction("DMBenhVien", "Admin");
            }
            try
            {
                DataModel db = new DataModel();

                // Lấy tên tệp
                string namefile = Path.GetFileName(hinhanh.FileName);

                // Đường dẫn để lưu tệp
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                string filePath = Path.Combine(uploadsFolder, namefile);

                // Lưu tệp
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    hinhanh.CopyTo(stream);
                }
            db.get($"EXEC AddBenhVien N'{tenbv}','{namefile}';");
            TempData["Message"] = "Thêm bệnh viện thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction("DMBenhVien", "Admin");
        }

        public IActionResult TimBenhVien(string mabv) 
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC sp_TimBenhVienTheoMa " + mabv + ";");
            return View();
        }

        [HttpPost] 
        public IActionResult SuaBenhVien(string mabv, string tenbv, IFormFile hinhanh)
        {
            if (hinhanh == null || hinhanh.Length == 0)
            {
                TempData["Error"] = "Vui lòng tải lên hình ảnh hợp lệ!";
                return RedirectToAction("DMBenhVien", "Admin");
            }

            try
            {
                DataModel db = new DataModel();

                // Lấy tên tệp
                string namefile = Path.GetFileName(hinhanh.FileName);

                // Đường dẫn để lưu tệp
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                string filePath = Path.Combine(uploadsFolder, namefile);

                // Lưu tệp
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    hinhanh.CopyTo(stream);
                }
            db.get($"EXEC sp_SuaBenhVien {mabv}, N'{tenbv}', N'{namefile}';");
            TempData["Message"] = "Sửa bệnh viện thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction("DMBenhVien", "Admin");
        }

        [HttpPost]
        public IActionResult XoaBenhVien(string mabv) 
        {
            DataModel db = new DataModel();
            try
            {
                // Gọi stored procedure để xóa khoa bệnh
                db.get("EXEC sp_XoaBenhVien " + mabv + ";");
                TempData["Message"] = "Xóa bệnh viện thành công!";
            }
            catch (Exception)
            {
                // Nếu xảy ra lỗi, thông báo lỗi liên quan đến khóa ngoại
                TempData["Error"] = "Bệnh viện được chọn không xóa được!!!!";
            }
            return RedirectToAction("DMBenhVien", "Admin");
        }
        // End Bệnh viện

        // Bệnh nhân
        public IActionResult HoSoBenhNhan()
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC GetPatientsWithoutDoctor");
            return View();
        }

        public IActionResult ChiTietBenhNhan(string id)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC GetPatientDetailsByUser_Id " +id+ ";" );
            return View();
        }

        public IActionResult SreachBenhNhan(string tenbn)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC SearchPatient N'" +tenbn+ "';" );
            return View();
        }


        [HttpPost]
        public IActionResult XoaBenhNhan(string mabn) 
        {
            DataModel db = new DataModel();
            try
            {
                // Gọi stored procedure để xóa khoa bệnh
                db.get("EXEC DeletePatientById " + mabn + ";");
                TempData["Message"] = "Xóa bệnh nhân thành công!";
            }
            catch (Exception)
            {
                // Nếu xảy ra lỗi, thông báo lỗi liên quan đến khóa ngoại
                TempData["Error"] = "Bệnh nhân được chọn không xóa được!!!!";
            }
            return RedirectToAction("HoSoBenhNhan", "Admin");
        }

        // End Bệnh nhân


        // Bác sĩ
        public IActionResult HoSoBacSi(string mapq)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC GetDoctorDetailsByRoleId " +3+ ";");
            return View();
        }

        public IActionResult ChiTietBacSi(string id)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC GetDoctorDetailsByUser_Id " +id+ ";" );
            return View();
        }

        public IActionResult SreachBacSi(string tenbs)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC SearchDoctorByName N'" +tenbs+ "';" );
            return View();
        }

        [HttpPost]
        public IActionResult XoaBacSi(string mabs) 
        {
            DataModel db = new DataModel();
            try
            {
                // Gọi stored procedure để xóa khoa bệnh
                db.get("EXEC DeleteDoctor " + mabs + ";");
                TempData["Message"] = "Xóa bác sĩ thành công!";
            }
            catch (Exception)
            {
                // Nếu xảy ra lỗi, thông báo lỗi liên quan đến khóa ngoại
                TempData["Error"] = "Bác sĩ được chọn không xóa được!!!!";
            }
            return RedirectToAction("HoSoBenhNhan", "Admin");
        }

        public IActionResult DonDangKyBacSi()
        {
            DataModel db = new DataModel();
            ViewBag.listDDK = db.get("EXEC GetPatientsByDoctor");
            return View();
        }

        public IActionResult ChiTietDonDangKy(string mabs) 
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC GetDoctorDetails " + mabs + ";");
            return View();
        }

        [HttpPost]
        public IActionResult DuyetBacSi(string mabs) {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC ConfirmDoctor " + mabs + ";");
            return RedirectToAction("DonDangKyBacSi", "Admin");
        }

        [HttpPost]
        public IActionResult HuyDonDangKy(string mabs)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC DeleteDoctorRegistration " + mabs + ";");
            return RedirectToAction("DonDangKyBacSi", "Admin");
        }

        // End Bác sĩ

        //Lịch khám
        public IActionResult LichKhamBenh()
        {
            DataModel db = new DataModel();
            ViewBag.listLH = db.get("EXEC GetAppointmentDetails");
            return View();
        }

        //End Lịch khám

        //Đánh giá
        public IActionResult DanhGiaPhanHoi() 
        {
            DataModel db = new DataModel();
            ViewBag.listDG = db.get("EXEC GetAllRatings ");
            return View();
        }

        //End Đánh giá

        //Thống kê
        public IActionResult ThongKe() 
        {
            DataModel db = new DataModel();
            ViewBag.listTKLK = db.get("EXEC ThongKeLuotKhamTheoThang ");
            ViewBag.listTKDT = db.get("EXEC ThongKeDoanhThuKhachTheoThang ");
            return View();
        }

        //End Thống kê

        //Thông báo
        public IActionResult ThongBaoBV()
        {
            DataModel db = new DataModel();
            ViewBag.listTB = db.get("EXEC LayDanhSachThongBao ");
            ViewBag.listND = db.get("SELECT * from NGUOIDUNG ");
            return View();
        }

        [HttpPost]
        public IActionResult GuiThongBao(string tieudetb, string noidungtb, string thoigiantb, string mand)
        {
            try
            {
                // Chuyển đổi thời gian từ chuỗi thành kiểu DateTime
                DateTime parsedTime = DateTime.Parse(thoigiantb);
                
                // Thực thi câu lệnh SQL để lưu thông báo
                DataModel db = new DataModel();
                db.get($"EXEC GuiThongBaoChoNguoiDung N'{tieudetb}', N'{noidungtb}', '{parsedTime}', '{mand}'");
                
                TempData["Message"] = "Thông báo đã được gửi thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi gửi thông báo: " + ex.Message;
            }

            return RedirectToAction("ThongBaoBV", "Admin");
        }


        [HttpPost]
        public IActionResult XoaThongBao(string matb)
        {
            DataModel db = new DataModel();
            try
            {
                // Gọi stored procedure để xóa khoa bệnh
                db.get("EXEC XoaThongBao " + matb + ";");
                TempData["Message"] = "Xóa thông báo thành công!";
            }
            catch (Exception)
            {
                
                TempData["Error"] = "Thông báo được chọn không xóa được!!!!";
            }
            return RedirectToAction("ThongBaoBV", "Admin");
        }

        //End Thông báo
        

        //Thanh toán
        public IActionResult DSThanhToan()
        {
            DataModel db = new DataModel();
            ViewBag.litsTT = db.get("EXEC LayDanhSachThanhToan");
            return View();
        }

        [HttpPost]       
        public IActionResult HoanPhiKham(string matt)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC HoanPhiKham " +matt);
            return RedirectToAction("DSThanhToan", "Admin");
        }

        //End Thanh toán

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(); // Trả về view lỗi mặc định 'Error.cshtml'
        }
    }
}