using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using todo.core.Services;
using todo.Models;

namespace todo.Controllers
{
	public class ItemController : Controller
	{
		private static IDocumentDbService _dbService;

		public ItemController(IDocumentDbService dbService)
		{
			_dbService = dbService;
		}

		[ActionName("Index")]
		public async Task<ActionResult> IndexAsync()
		{
			var items = await _dbService.GetItems(d => !d.Completed);
			return View(items);
		}

		[ActionName("Create")]
		public async Task<ActionResult> CreateAsync()
		{
			return View();
		}

		[HttpPost]
		[ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateAsync(Item item)
		{
			if (ModelState.IsValid)
			{
				await _dbService.AddItem(item);
				return RedirectToAction("Index");
			}

			return View(item);
		}

		[HttpPost]
		[ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditAsync(Item item)
		{
			if (ModelState.IsValid)
			{
				await _dbService.UpdateItem(item.Id, item);
				return RedirectToAction("Index");
			}

			return View(item);
		}

		[ActionName("Edit")]
		public async Task<ActionResult> EditAsync(string id)
		{
			if (id == null)
			{
				return new StatusCodeResult((int)HttpStatusCode.BadRequest);
			}

			Item item = await _dbService.GetItem(id);
			if (item == null)
			{
				return NotFound();
			}

			return View(item);
		}

		[ActionName("Details")]
		public async Task<ActionResult> DetailsAsync(string id)
		{
			if (id == null)
			{
				return new StatusCodeResult((int)HttpStatusCode.BadRequest);
			}

			Item item = await _dbService.GetItem(id);
			if (item == null)
			{
				return NotFound();
			}

			return View(item);
		}

		[ActionName("Delete")]
		public async Task<ActionResult> DeleteAsync(string id)
		{
			if (id == null)
			{
				return new StatusCodeResult((int)HttpStatusCode.BadRequest);
			}

			Item item = await _dbService.GetItem(id);
			if (item == null)
			{
				return NotFound();
			}

			return View(item);

		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteAsync(Item item)
		{
			if (ModelState.IsValid)
			{
				await _dbService.DeleteItem(item.Id);
				return RedirectToAction("Index");
			}

			return View(item);
		}
	}
}