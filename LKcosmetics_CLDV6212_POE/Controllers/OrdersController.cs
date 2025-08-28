using LKcosmetics_CLDV6212_POE.Models;
using LKcosmetics_CLDV6212_POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LKcosmetics_CLDV6212_POE.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;

        public OrdersController(
            TableStorageService tableStorageService,
            QueueService queueService,
            FileService fileService)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

      
        [HttpPost]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tableStorageService.DeleteOrderAsync(partitionKey, rowKey);
            await _queueService.EnqueueMessageAsync($"Deleted order {rowKey}");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(Order order, IFormFile? file)
        {
            order.PartitionKey = "Orders";
            order.RowKey = Guid.NewGuid().ToString();
            order.Status = "Pending";

            await _tableStorageService.AddOrderAsync(order);
            await _queueService.EnqueueMessageAsync(
                $"Processing order {order.RowKey} for customer {order.CustomerId}"
            );

            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                await _fileService.UploadFileAsync(order.RowKey, file.FileName, stream);

                order.FileName = file.FileName;
                await _tableStorageService.UpdateOrderAsync(order); 
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddOrder()
        {
            return View();
        }

        
        public async Task<IActionResult> DownloadFile(string orderId, string fileName)
        {
            try
            {
                var stream = await _fileService.DownloadFileAsync(orderId, fileName);
                if (stream == null)
                    return NotFound();

                return File(stream, "application/octet-stream", fileName);
            }
            catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "ParentNotFound")
            {
                return NotFound("The folder for this order does not exist.");
            }
        }

     
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string partitionKey, string rowKey, string newStatus)
        {
            var order = await _tableStorageService.GetOrderAsync(partitionKey, rowKey);
            if (order != null)
            {
                order.Status = newStatus;
                await _tableStorageService.UpdateOrderAsync(order);
                await _queueService.EnqueueMessageAsync($"Order {rowKey} status updated to {newStatus}");
            }
            return RedirectToAction("Index");
        }
            [HttpGet]
            public async Task<IActionResult> Edit(string partitionKey, string rowKey)
{
             var order = await _tableStorageService.GetOrderAsync(partitionKey, rowKey);
             if (order == null) return NotFound();
              return View("EditOrder", order);
}

             [HttpPost]
             public async Task<IActionResult> Edit(Order order)
{
             if (!ModelState.IsValid) return View(order);
              await _tableStorageService.UpdateOrderAsync(order);
              return RedirectToAction("Index");
}
    }
}
