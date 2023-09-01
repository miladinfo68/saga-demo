using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using CommonService.Dtos;
using OrderService.Extensions;
using OrderService.Models;

namespace OrderService.ApiEndPoints;
public static class OrderEndPoints
{
    //https://dev.to/moe23/net-7-preview-4-minimal-api-multiple-result-type-route-groups-3k74
    public static IEndpointRouteBuilder AddOrderEndPoints(this IEndpointRouteBuilder routes)
    {

        routes.MapGet("/v1/Orders", GetAllOrders);
        routes.MapGet("/v1/Orders/{id}", GetOrderById);
        routes.MapPost("/v1/Orders", AddOrder);

        return routes;
    }

    private static async Task<Ok<List<Order>>> GetAllOrders(OrderDbContext orderDb)
    {
        return TypedResults.Ok(await (orderDb.Orders.ToListAsync()));
    }

    private static async Task<Ok<Order>> GetOrderById(OrderDbContext orderDb, int id)
    {
        return TypedResults.Ok(await (orderDb.Orders.FindAsync(id)));
    }

    private static async Task<Ok<int>> AddOrder(OrderDbContext orderDb, AddOrderDto dto)
    {
        await using var orderTransaction = await orderDb.Database.BeginTransactionAsync();
        try
        {
            var targetProducts = await orderDb.Products
                .Where(w => dto.OrderItemsDto.Select(s => s.ProductId).Any(a => a == w.Id)).ToListAsync();

            if (!targetProducts.Any())  return TypedResults.Ok(-1);
            
            //Add Order
            var order = dto.AsOrder(dto.OrderItemsDto);
           
            //Add Order Items
            var orderItems = dto.OrderItemsDto.AsOrderItems(order.Id).ToArray();
            order.OrderItems = orderItems;
            await orderDb.Orders.AddAsync(order);
            await orderDb.SaveChangesAsync();
            
            //Add OrderOutbox 
            var orderOutbox = order.AsAddedOrderOutbox(dto.OrderItemsDto);
            await orderDb.OrderOutbox.AddAsync(orderOutbox);
            await orderDb.SaveChangesAsync();

            await orderTransaction.CommitAsync();

            return TypedResults.Ok(order.Id);
        }
        catch (Exception ex)
        {
            await orderTransaction.RollbackAsync();
            return TypedResults.Ok(-1);
        }

    }
}

