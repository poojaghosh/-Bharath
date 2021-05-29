    $(document).ready(function () {

            // Welcome modal
            $("#welcomemodal").modal();

            // Search Modal
            $(".main .myDiv").hide();
            $(".main .myDiv:first-child").show();
            $(".myLink").click(function () {
                $(".myDiv").hide();
                var divId = $(this).attr("data-show");
                $("." + divId).show();
            });


        });
 