﻿using BLLProject.UnitOfWorkPattern;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PLProject.Services.ProductServcie;
using PLProject.Services.ServiceProviderService;
using PLProject.ViewModel.ProductViewModel;

namespace PLProject.Controllers
{

    public class ProductController(IProductService _productService, IServiceProviderService _serviceProviderService) : Controller
    {
        // GET: Product/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading products: {ex.Message}";
                return View(new List<GetAllProductViewModel>());
            }
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                var serviceProviders = await _serviceProviderService.GetAllServiceProvidersAsync();
                ViewBag.ServiceProviders = new SelectList(serviceProviders, "Id", "Name", product.ServiceProviderId);

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading product details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Product/Filter
        [HttpGet]
        public async Task<IActionResult> Filter()
        {
            try
            {
                var filterViewModel = new FilterProductViewModel();
                await PopulateServiceProvidersDropdownAsync();
                
                return View(filterViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading filter page: {ex.Message}";
                return View(new FilterProductViewModel());
            }
        }

        // POST: Product/Filter
        [HttpPost]
        public async Task<IActionResult> Filter(FilterProductViewModel filterViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateServiceProvidersDropdownAsync(filterViewModel.ServiceProviderId);
                    return View(filterViewModel);
                }

                var filteredProducts = await _productService.GetFilteredProductsAsync(
                    filterViewModel.MinPrice,
                    filterViewModel.MaxPrice,
                    filterViewModel.FromDate,
                    filterViewModel.ToDate,
                    filterViewModel.ServiceProviderId
                );

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_ProductList", filteredProducts);
                }

                await PopulateServiceProvidersDropdownAsync(filterViewModel.ServiceProviderId);
                filterViewModel.Products = filteredProducts.ToList();
                return View(filterViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error filtering products: {ex.Message}";
                await PopulateServiceProvidersDropdownAsync(filterViewModel.ServiceProviderId);
                return View(filterViewModel);
            }
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateServiceProvidersDropdownAsync();
                
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading create page: {ex.Message}";
                return View();
            }
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel productViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var createdProduct = await _productService.CreateProductAsync(productViewModel);
                    TempData["SuccessMessage"] = "Product created successfully!";
                    return RedirectToAction(nameof(Details), new { id = createdProduct.Id });
                }

                // If we got this far, something failed; redisplay form
                await PopulateServiceProvidersDropdownAsync(productViewModel.ServiceProviderId);
                return View(productViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating product: {ex.Message}");
                await PopulateServiceProvidersDropdownAsync(productViewModel.ServiceProviderId);
                return View(productViewModel);
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                await PopulateServiceProvidersDropdownAsync(product.ServiceProviderId);
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading product for editing: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetialProductViewModel productViewModel)
        {
            try
            {
                if (id != productViewModel.Id)
                {
                    TempData["ErrorMessage"] = "Product ID mismatch.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    await _productService.UpdateProductAsync(productViewModel);
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = productViewModel.Id });
                }

                await PopulateServiceProvidersDropdownAsync(productViewModel.ServiceProviderId);
                return View(productViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating product: {ex.Message}");
                await PopulateServiceProvidersDropdownAsync(productViewModel.ServiceProviderId);
                return View(productViewModel);
            }
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading product for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                TempData["SuccessMessage"] = "Product deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting product: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private async Task PopulateServiceProvidersDropdownAsync(object selectedProvider = null)
        {
            var serviceProviders = await _serviceProviderService.GetAllServiceProvidersAsync();
            ViewBag.ServiceProviders = new SelectList(serviceProviders, "Id", "Name", selectedProvider);
        }
    }
}