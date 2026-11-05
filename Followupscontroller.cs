
@model AcxiomCRM.Models.Opportunity

<div class="container py-4">
    <h2>Delete Opportunity</h2>
    <div class="card shadow-sm p-4 mt-3">
        <h5 class="text-danger">Are you sure you want to delete this opportunity?</h5>
        <p><strong>Name:</strong> @Model.OpportunityName</p>
        <p><strong>Amount:</strong> ₹@Model.Amount.ToString("N2")</p>
        <p><strong>Stage:</strong> @Model.Stage</p>

        <form asp-action="Delete" asp-route-id="@Model.OpportunityId" method="post">
            @Html.AntiForgeryToken()
            <button type="submit" class="btn btn-danger">Confirm Delete</button>
            <a asp-action="Index" class="btn btn-secondary">Cancel</a>
        </form>
    </div>
</div>
