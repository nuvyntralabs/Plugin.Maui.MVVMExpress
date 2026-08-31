using Plugin.Maui.MVVMExpress.Samples.Crud;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Crud;

public sealed class ProductCrudTests
{
    [Fact]
    public async Task List_Initialize_LoadsAndBatches()
    {
        var (catalog, hub, _, _) = SampleHarness.Core();
        var vm = new ProductListViewModel(catalog, hub);
        await vm.InitializeAsync();
        Assert.True(vm.Products.IsSuccess);
        Assert.Equal(5, vm.Items.Count);
        Assert.Equal(5, vm.Products.Data?.Count);
    }

    [Fact]
    public async Task List_Empty_SetsEmpty()
    {
        var (catalog, hub, _, _) = SampleHarness.Core();
        catalog.Seed([]);
        var vm = new ProductListViewModel(catalog, hub);
        await vm.InitializeAsync();
        Assert.True(vm.Products.IsEmpty);
        Assert.Empty(vm.Items);
    }

    [Fact]
    public async Task List_Failure_SetsError()
    {
        var (catalog, hub, _, _) = SampleHarness.Core();
        catalog.FailNext = true;
        var vm = new ProductListViewModel(catalog, hub);
        await Assert.ThrowsAsync<InvalidOperationException>(() => vm.InitializeAsync());
        Assert.True(vm.Products.HasError);
    }

    [Fact]
    public async Task List_Delete_PublishesHub()
    {
        var (catalog, hub, _, _) = SampleHarness.Core();
        var seen = 0;
        var listener = new ProductListViewModel(catalog, hub);
        hub.Subscribe<ProductListViewModel, ProductsChanged>(listener, (_, msg) => seen = msg.Count);
        var vm = new ProductListViewModel(catalog, hub);
        await vm.InitializeAsync();
        await vm.DeleteCommand.ExecuteAsync(1);
        Assert.Equal(4, vm.Items.Count);
        Assert.Equal(4, seen);
    }

    [Fact]
    public async Task Edit_Save_CreatesProduct()
    {
        var (catalog, _, errors, busy) = SampleHarness.Core();
        var vm = new ProductEditViewModel(catalog, errors, busy);
        vm.Name = "Americano";
        vm.Price = 3.00m;
        await vm.SaveCommand.ExecuteAsync();
        Assert.True(vm.LastSave?.IsSuccess);
        Assert.True(vm.Id > 0);
        Assert.Equal(ViewModelStatus.Success, vm.Status);
        Assert.False(busy.IsBusy);
    }

    [Fact]
    public void Edit_Save_EmptyName_CannotExecute()
    {
        var (catalog, _, errors, busy) = SampleHarness.Core();
        var vm = new ProductEditViewModel(catalog, errors, busy);
        Assert.False(vm.SaveCommand.CanExecute(null));
        vm.Name = "X";
        Assert.True(vm.SaveCommand.CanExecute(null));
    }

    [Fact]
    public async Task Edit_Save_Failure_GoesToErrorSink()
    {
        var (catalog, _, errors, busy) = SampleHarness.Core();
        catalog.FailNext = true;
        var vm = new ProductEditViewModel(catalog, errors, busy) { Name = "X", Price = 1.00m };
        await Assert.ThrowsAsync<InvalidOperationException>(() => vm.SaveCommand.ExecuteAsync());
        Assert.Equal(ViewModelStatus.Error, vm.Status);
        Assert.Single(errors.Errors);
        Assert.False(busy.IsBusy);
    }

    [Fact]
    public async Task Edit_Save_InvalidPrice_FailsValidation()
    {
        var (catalog, _, errors, busy) = SampleHarness.Core();
        var vm = new ProductEditViewModel(catalog, errors, busy) { Name = "Free", Price = 0m };
        await vm.SaveCommand.ExecuteAsync();
        Assert.False(vm.LastSave?.IsSuccess);
        Assert.Equal("E_VALIDATION", vm.LastSave?.Error?.Code);
        Assert.Equal(ViewModelStatus.Error, vm.Status);
        Assert.Single(errors.Errors);
    }

    [Fact]
    public async Task Appear_LoadsOnce()
    {
        var (catalog, hub, _, _) = SampleHarness.Core();
        var vm = new ProductListViewModel(catalog, hub);
        await vm.OnAppearingAsync();
        await vm.OnAppearingAsync();
        Assert.Equal(5, vm.Items.Count);
        Assert.True(vm.Products.IsSuccess);
    }
}
