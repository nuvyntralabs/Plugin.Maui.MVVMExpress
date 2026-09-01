using Plugin.Maui.MVVMExpress.Forms;
using Plugin.Maui.MVVMExpress.Outcome;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Forms;

public sealed class FormFieldAndSubmitTests
{
    [Fact]
    public void SetErrors_RaisesErrorAndHasError()
    {
        var field = new FormField<string>("Password");
        var names = new List<string>();
        field.PropertyChanged += (_, e) => names.Add(e.PropertyName ?? "");
        field.SetErrors([new ValidationMessage("Password", "Too short")]);
        Assert.True(field.HasError);
        Assert.Equal("Too short", field.Error);
        Assert.Contains(nameof(FormField<string>.Error), names);
        Assert.Contains(nameof(FormField<string>.HasError), names);
    }

    [Fact]
    public async Task DirtyConfirm_UsesDialogs()
    {
        var dialogs = new FakeDialogs { ConfirmResult = true };
        var form = new ConfirmForm(dialogs);
        form.Title = "x";
        Assert.True(await form.CanNavigateAwayAsync());
        dialogs.ConfirmResult = false;
        Assert.False(await form.CanNavigateAwayAsync());
        Assert.Contains(dialogs.Alerts, item => item.Contains("Discard", StringComparison.Ordinal));
    }

    [Fact]
    public async Task SubmitAsync_MarksClean_OnSuccess()
    {
        var form = new ConfirmForm(new FakeDialogs());
        form.Title = "saved";
        Assert.True(form.IsDirty);
        var result = await form.RunSubmitAsync();
        Assert.True(result.IsSuccess);
        Assert.False(form.IsDirty);
    }

    [Fact]
    public void MustMatch_ReturnsMessage_WhenDifferent()
    {
        var form = new ConfirmForm(new FakeDialogs());
        var error = form.Compare(new FormField<string>("Password", "a"), new FormField<string>("Confirm", "b"));
        Assert.Equal("Confirm", error?.PropertyName);
    }

    private sealed class ConfirmForm : FormViewModel
    {
        private readonly FormField<string> _title;

        public ConfirmForm(FakeDialogs dialogs)
            : base(dialogs: dialogs)
        {
            _title = Field("Title", "");
        }

        public string Title
        {
            get => _title.Value ?? "";
            set => _title.Value = value;
        }

        public Task<Plugin.Maui.MVVMExpress.Outcome.Outcome> RunSubmitAsync()
            => SubmitAsync(_ => Task.FromResult(Plugin.Maui.MVVMExpress.Outcome.Outcome.Success()));

        public ValidationMessage? Compare(FormField<string> left, FormField<string> right)
            => MustMatch(left, right);
    }
}
