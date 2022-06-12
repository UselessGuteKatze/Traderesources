$(function () {
    $(".select-gr-object-with-next-btn").each(function () {
        var $this = $(this);
        var searchCollectionUrl = $this.data("searchCollectionUrl");
        var queryInputPlaceholder = $this.data("searchQueryInputPlaceholder");
        
        var editorName = $(this).data("editorName");
        var $div = $("<div>");
        $this.append($div);

        var $btn = $("<input type='submit' class='btn btn-primary'>");
        $btn.prop("value", T("Далее"));
        $btn.prop("disabled", true);
        $this.append($btn);

        $div.searchCollectionWidget({
            searchCollectionUrl: searchCollectionUrl, 
            queryInputPlaceholder: queryInputPlaceholder,
            onQueryExecuted: function(result) {
                var validValue=null;
                if (result.length > 0 && result[0].Item && result[0].Item.SearchItemId) {
                    validValue = result[0].Item;
                }

                if(validValue) {
                    $this.data("value", validValue);
                    $btn.prop("disabled", false);
                } else {
                    $this.data("value", null);
                    $btn.prop("disabled", true);
                }
            }
        });

        $this.closest("form").submit(function(){
            var val = $this.data("value");
            if(val != null){
                var $input =  $("<input type='hidden' name='{0}'>".format(editorName));
                $input.val(val.SearchItemId);
                $(this).append($input);
            }
        });
    });
});