using Microsoft.AspNetCore.Mvc;
using PLProject.Services.ServiceProviderService;
using PLProject.ViewModel.ServiceProviderViewModel;

namespace PLProject.Controllers
{
    public class ServiceProviderController(IServiceProviderService _serviceProviderService) : Controller
    {
        // GET: ServiceProvider/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var serviceProviders = await _serviceProviderService.GetAllServiceProvidersAsync();
                return View(serviceProviders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading service providers: {ex.Message}";
                return View(new List<GetAllServiceProviderViewModel>());
            }
        }

        // GET: ServiceProvider/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var serviceProvider = await _serviceProviderService.GetServiceProviderByIdAsync(id);
                if (serviceProvider == null)
                {
                    TempData["ErrorMessage"] = "Service Provider not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(serviceProvider);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading service provider details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ServiceProvider/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceProvider/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceProviderViewModel serviceProviderViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var createdServiceProvider = await _serviceProviderService.CreateServiceProviderAsync(serviceProviderViewModel);
                    TempData["SuccessMessage"] = "Service Provider created successfully!";
                    return RedirectToAction(nameof(Details), new { id = createdServiceProvider.Id });
                }

                // If we got this far, something failed; redisplay form
                return View(serviceProviderViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating service provider: {ex.Message}");
                return View(serviceProviderViewModel);
            }
        }

        // GET: ServiceProvider/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var serviceProvider = await _serviceProviderService.GetServiceProviderByIdAsync(id);
                if (serviceProvider == null)
                {
                    TempData["ErrorMessage"] = "Service Provider not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(serviceProvider);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading service provider for editing: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ServiceProvider/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetailServiceProviderViewModel serviceProviderViewModel)
        {
            try
            {
                if (id != serviceProviderViewModel.Id)
                {
                    TempData["ErrorMessage"] = "Service Provider ID mismatch.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    await _serviceProviderService.UpdateServiceProviderAsync(serviceProviderViewModel);
                    TempData["SuccessMessage"] = "Service Provider updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = serviceProviderViewModel.Id });
                }

                return View(serviceProviderViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating service provider: {ex.Message}");
                return View(serviceProviderViewModel);
            }
        }

        // GET: ServiceProvider/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var serviceProvider = await _serviceProviderService.GetServiceProviderByIdAsync(id);
                if (serviceProvider == null)
                {
                    TempData["ErrorMessage"] = "Service Provider not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(serviceProvider);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading service provider for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ServiceProvider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _serviceProviderService.DeleteServiceProviderAsync(id);
                TempData["SuccessMessage"] = "Service Provider deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting service provider: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
