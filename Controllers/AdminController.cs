using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PayPal.Api;
using Project.Data;
using Project.Models;
using System.Reflection.Metadata;
using WebApplication6.Service;

namespace Project.Controllers
{

    public class AdminController : Controller
    {
        private readonly ShopContext _shopContext;
        private readonly ICloudinaryService _cloudinaryService;
        public AdminController(ShopContext shopContext, ICloudinaryService temp)
        {
            _shopContext = shopContext;
            _cloudinaryService = temp;
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult Index()
        {
            DateTime now = DateTime.Now;
            var x = _shopContext.Bills.Where(p => p.PurchaseDate.Year == now.Year).Where(p => p.BillStatus.Equals("3")).ToList();
            double total = 0;
            double totalMonth = 0;
            double totalDay = 0;
            SortedDictionary<int, double> myDictionary = new SortedDictionary<int, double>();

            foreach (var l in x)
            {
                if (myDictionary.ContainsKey(l.PurchaseDate.Month))
                {
                    myDictionary[l.PurchaseDate.Month] += l.TotalPrice;
                }
                else
                {
                    myDictionary.Add(l.PurchaseDate.Month, l.TotalPrice);
                }
                total += l.TotalPrice;
                if (l.PurchaseDate.Month == now.Month)
                {
                    totalMonth += l.TotalPrice;
                    if (l.PurchaseDate.Day == now.Day)
                        totalDay += l.TotalPrice;
                }
            }

            ViewData["total"] = total;
            ViewData["totalMonth"] = totalMonth;
            ViewData["totaday"] = totalDay;

            foreach (var l in x)
            {
                if (myDictionary.ContainsKey(l.PurchaseDate.Month))
                {
                    myDictionary[l.PurchaseDate.Month] += l.TotalPrice;
                }
                else
                {
                    myDictionary.Add(l.PurchaseDate.Month, l.TotalPrice);
                }
                total += l.TotalPrice;
                if (l.PurchaseDate.Month == now.Month)
                {
                    totalMonth += l.TotalPrice;
                    if (l.PurchaseDate.Day == now.Day)
                        totalDay += l.TotalPrice;
                }
            }


            var groupedBillDetails = _shopContext.BillDetails
                                       .GroupBy(billDetail => billDetail.ProductId)
                                       .Select(group => new
                                       {
                                           ProductId = group.Key,
                                           TotalQuantity = group.Sum(item => item.quantity),
                                       })
                                      .OrderByDescending(product => product.TotalQuantity)
                                      .Take(3)
                                      .ToList();
            ViewData["top1"] = "";
            ViewData["top11"] = "";
            ViewData["top2"] = "";
            ViewData["top22"] = "";
            ViewData["top3"] = "";
            ViewData["top33"] = "";
            var i = 1;
            foreach (var l in groupedBillDetails)
            {
                foreach (var ls in _shopContext.Products.ToList())
                {
                    if (ls.ProductId == l.ProductId)
                    {
                        Console.WriteLine(ls.ProductName);
                        ViewData[$"top{i}"] = ls.ProductName;
                        ViewData[$"top{i}{i}"] = l.TotalQuantity;
                        i++;
                    }
                }
            }

            ViewData["pCount"] = _shopContext.Products.ToList().Count();
            ViewData["uCount"] = _shopContext.Users.ToList().Count();
            ViewData["yCount"] = _shopContext.SubCategory.Count();
            ViewData["rCount"] = _shopContext.Bills.ToList().Count;

            return View(myDictionary);
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
		public IActionResult cfFeedback()
		{
			var feedbacks = _shopContext.Feedbacks
				.Include(i => i.User)
				.ToList();

			return View(feedbacks);
		}

		[Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult confirmFeedback(int feedbackId)
        {
            var feedback = _shopContext.Feedbacks.FirstOrDefault(f => f.FeedbackId == feedbackId);
            if (feedback != null)
            {
                feedback.FeedbackStatus = "1";
                _shopContext.SaveChanges();
            }

            return RedirectToAction("cfFeedback", "admin");
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult deleteFb(int feedbackId)
        {
            var feedback = _shopContext.Feedbacks.FirstOrDefault(f => f.FeedbackId == feedbackId);
            if (feedback != null)
            {
                _shopContext.Feedbacks.Remove(feedback);
                _shopContext.SaveChanges();
            }
            return RedirectToAction("cfFeedback", "admin");
        }

        //display product list
        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult DashProduct()
        {
            List<Product> products = _shopContext.Products.ToList();
            return View(products);
        }

        //search product by name
        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult searchProductByName(string name)
        {
            List<Product> products = null;

            if (name != null)
            {
                products = _shopContext.Products.Where(x => x.ProductName.Contains(name)).ToList();
            }
            else

                products = _shopContext.Products.ToList();

            return View(products);
        }

        [HttpPost]
        //add produc from excel file
        [Authorize(Roles = "Seller, Admin, Marketing")]

        public IActionResult upExcelProduct(IFormFile fileExcel)
        {

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (fileExcel != null && fileExcel.Length > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(fileExcel.OpenReadStream()))
                    {
                        try
                        {
                            var worksheet = package.Workbook.Worksheets[0];


                            int rowCount = worksheet.Dimension.Rows;
                            for (int row = 2; row <= rowCount; row++) // Start from the second row (excluding the header)
                            {
                                var product = new Product();
                                //name, description, category, date, discout,price image, avaiable, homestatus
                                if (worksheet.Cells[row, 1].Value == null)
                                {
                                    TempData["checkExcel"] = "add failed";
                                    return Redirect("DashProduct");
                                }
                                else
                                {
                                    product.ProductName = worksheet.Cells[row, 1].Value.ToString(); // Read value from the Name column (column 2)
                                }


                                if (worksheet.Cells[row, 2].Value == null)
                                {
                                    TempData["checkExcel"] = "add failed";
                                    return Redirect("DashProduct");
                                }
                                else
                                {
                                    product.ProductDescription = worksheet.Cells[row, 2].Value.ToString();
                                }


                                if (worksheet.Cells[row, 3].Value == null)
                                {
                                    TempData["checkExcel"] = "add failed";
                                    return Redirect("DashProduct");
                                }
                                else
                                {
                                    string cellValue = worksheet.Cells[row, 3].Value.ToString();
                                    if (int.TryParse(cellValue, out int result))
                                    {
                                        product.SubCategoryID = result;
                                    }
                                    else
                                    {
                                        TempData["checkExcel"] = "add failed";
                                        return Redirect("DashProduct");
                                    }

                                }


                                product.ImportDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));


                                if (worksheet.Cells[row, 4].Value == null)
                                {
                                    TempData["checkExcel"] = "add failed";
                                    return Redirect("DashProduct");
                                }
                                else
                                {
                                    string cellValue = worksheet.Cells[row, 4].Value.ToString();
                                    if (double.TryParse(cellValue, out double result))
                                    {
                                        product.Discount = result;
                                    }
                                    else
                                    {
                                        TempData["checkExcel"] = "add failed";
                                        return Redirect("DashProduct");
                                    }

                                }


                                if (worksheet.Cells[row, 5].Value == null)
                                {
                                    TempData["checkExcel"] = "add failed";
                                    return Redirect("DashProduct");
                                }
                                else
                                {
                                    string cellValue = worksheet.Cells[row, 5].Value.ToString();
                                    if (double.TryParse(cellValue, out double result))
                                    {
                                        product.ProductPrice = result;
                                    }
                                    else
                                    {
                                        TempData["checkExcel"] = "add failed";
                                        return Redirect("DashProduct");
                                    }

                                }


                                if( worksheet.Cells[row, 6].Value == null)
                                {
                                    TempData["checkExcel"] = "add failed";
                                    return Redirect("DashProduct");
                                }
                                else
                                {
                                    product.ImageMain = worksheet.Cells[row, 6].Value.ToString();

                                }
                              
                                product.IsAvailble = false;
                                product.HomeStatus = false;
                                _shopContext.Products.Add(product);
                                _shopContext.SaveChanges();
                            }
                            TempData["checkExcel"] = "add successfull";
                        }
                        catch (Exception ex)
                        {
                            TempData["checkExcel"] = "add failed";
                            return Redirect("DashProduct");
                        }

                    }
                }
                catch
                {
                    TempData["checkExcel"] = "add failed";
                    return Redirect("DashProduct");
                }
            }
            return Redirect("DashProduct");
        }

        //Delete product
        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult delProd(string productId)
        {

            var product = _shopContext.Products.FirstOrDefault(p => p.ProductId == Int32.Parse(productId));
            if (product != null)
            {
                _shopContext.Products.Remove(product);
                _shopContext.SaveChanges();
                return Redirect("DashProduct");
            }
            

             return Redirect("Index");
           
        }


        //change product's Home Status
        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult changeHomeStatus(string pid)
        {

            var product = _shopContext.Products.FirstOrDefault(p => p.ProductId == Int32.Parse(pid));
            if (product != null)
            {
                if (product.HomeStatus == true)
                {
                    product.HomeStatus = false;
                    _shopContext.Update(product);
                    _shopContext.SaveChanges();
                    return Redirect("DashProduct");
                }
                else
                {
                    product.HomeStatus = true;
                    _shopContext.Update(product);
                    _shopContext.SaveChanges();
                    return Redirect("DashProduct");
                }
            }

            return Redirect("Index");

        }

        //create product
        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult CreateProduct()
        {

            List<SubCategory> subcate = _shopContext.SubCategory.ToList();

            return View(subcate);
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        [HttpPost]
        public IActionResult CreateProduct(IFormFile ImageUrl, Product product)
        {

            var imageURL = _cloudinaryService.UploadImage(ImageUrl, "MainImageProduct");

            product.ImageMain = imageURL;
            _shopContext.Add(product);
            _shopContext.SaveChanges();

            return Redirect("DashProduct");
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult ViewDetailProduct(int productId)
        {
            Product product = _shopContext.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                return View(product);
            }
            else
            {
                TempData["checked"] = "";
                return Redirect("/Admin/DashProduct");
            }
            
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult UpdateProduct(int productId)
        {
            List<SubCategory> subcate = _shopContext.SubCategory.ToList();
            Product product = _shopContext.Products.FirstOrDefault(x => x.ProductId == productId);


            if (product != null)
            {
                ViewBag.Product = product;

                return View(subcate);
            }
            else
            {
                TempData["checked"] = "";
                return Redirect("/Admin/DashProduct");
            }

        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        [HttpPost]
        public IActionResult UpdateProduct(IFormFile ImageUrl, Product updateProd)
        {
            Product product = _shopContext.Products.FirstOrDefault(x => x.ProductId == updateProd.ProductId);

            if (product != null)

            {
                product.ProductName = updateProd.ProductName;
                product.ProductDescription = updateProd.ProductDescription;
                product.SubCategoryID = updateProd.SubCategoryID;
                product.ImportDate = updateProd.ImportDate;
                product.ProductPrice = updateProd.ProductPrice;
                product.Discount = updateProd.Discount;
                product.HomeStatus = updateProd.HomeStatus;
                product.IsAvailble = updateProd.IsAvailble;
                if (ImageUrl != null)
                {
                    product.ImageMain = _cloudinaryService.UploadImage(ImageUrl, "MainImageProduct");
                }

                _shopContext.Update(product);
                _shopContext.SaveChanges();
                return Redirect("DashProduct");
            }





            return Redirect("DashProduct");
        }




        [Authorize(Roles = "Seller, Admin, Marketing")]
        //Detail Product
        public IActionResult ViewDetailProd(int productId)
        {
            Product product = _shopContext.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                List<ProductDetails> pr = _shopContext.productdetails.Where(x => x.productId == productId).ToList();
                ViewBag.ProductDetails = pr;
                List<ImageProduct> imgProd = _shopContext.ImageProducts.Where(x => x.ProductId == productId).ToList();
                ViewBag.ImageProducts = imgProd;
            }
            else
            {
                TempData["checked"] = "";
                return Redirect("/Admin/DashProduct");
            }
            return View(product);
        }

        //check product detail is exist
        private Boolean checkDetailExist(ProductDetails detail)
        {
            if (detail != null)
            {
                List<ProductDetails> pr = _shopContext.productdetails.Where(x => x.productId == detail.productId).ToList();
                foreach (ProductDetails a in pr)
                {
                    if (a.size == detail.size && a.color.Equals(detail.color))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        [Authorize(Roles = "Seller, Admin, Marketing")]
        [HttpPost]
        public IActionResult CreateProductDetail(ProductDetails details)
        {
            Boolean test = checkDetailExist(details);
            string mess;
            if (test)
            {
                _shopContext.Add(details);
                _shopContext.SaveChanges();
                ChangeAvaiableProduct(details.productId);
                mess = "create successfully";
            }
            else
            {
                mess = "create failed";
            }
            TempData["mess"] = mess;
            return Redirect($"ViewDetailProd?productId={details.productId}");
        }

        //delete detail product
        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult DelDetailProduct(int productDetailId)
        {
            var detailProd = _shopContext.productdetails.FirstOrDefault(x => x.productDetailId == productDetailId);
            int prodId = detailProd.productId;
            _shopContext.Remove(detailProd);
            _shopContext.SaveChanges();
            ChangeAvaiableProduct(prodId);

            TempData["mess"] = " delete sucessfully";
            return Redirect($"ViewDetailProd?productId={prodId}");
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        [HttpPost]
        public IActionResult CreateImageProduct(IFormFile ImageUrl, ImageProduct imageProduct)
        {


            //return Redirect($"ViewDetailProd?productId={imageProduct.ProductId}");


            var imageURL = _cloudinaryService.UploadImage(ImageUrl, "ImageProduct");

            imageProduct.ImageURL = imageURL;
            _shopContext.Add(imageProduct);
            _shopContext.SaveChanges();
            TempData["mess"] = " create sucessfully";
            return Redirect($"ViewDetailProd?productId={imageProduct.ProductId}");
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        //delete Image Product
        public IActionResult DelImageProduct(int ImageProductId)
        {
            ImageProduct img = _shopContext.ImageProducts.FirstOrDefault(x => x.ImageProductId == ImageProductId);
            int prodId = img.ProductId;
            _shopContext.Remove(img);

            _shopContext.SaveChanges();
           

            TempData["mess"] = "delete sucessfully";
            return Redirect($"ViewDetailProd?productId={prodId}");
        }

        private void ChangeAvaiableProduct(int productId)
        {
            var detailList = _shopContext.productdetails.Where(x => x.productId == productId && x.quantity > 0).ToList();
            var product = _shopContext.Products.FirstOrDefault(x => x.ProductId == productId);
            if (detailList == null || detailList.Count == 0  )
            {
                product.IsAvailble = false;
                
            }
            else
            {
                product.IsAvailble = true;
            }
            _shopContext.Update(product);
            _shopContext.SaveChanges();
        }

    }
}
