<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Shipwreck.ImasCGImages.WebApi.Views.Image.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery-2.2.3.min.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/knockout-3.4.0.js") %>"></script>
</head>
<body>
    <form action="<%= Url.Action("search", "api") %>" method="get" target="rawJson">
    <fieldset>
        <legend>ヘッドライン</legend>
        <label for="headline">headline</label>
        <input id="headline" name="headline" type="text" />

        <label for="headlineOperator">headlineOperator</label>
        <select id="headlineOperator" name="headlineOperator">
            <option value="Equal">Equal</option>
            <option value="Contains" selected="selected">Contains (default)</option>
        </select>

    </fieldset>
    <fieldset>
        <legend>名前</legend>
        <label for="name">name</label>
        <input id="name" name="name" type="text" />

        <label for="nameOperator">nameOperator</label>
        <select id="nameOperator" name="nameOperator">
            <option value="Equal" selected="selected">Equal (default)</option>
            <option value="Contains">Contains</option>
        </select>
         
        <label for="kana">kana</label>
        <input id="kana" name="kana" type="text" />

        <label for="kanaOperator">kanaOperator</label>
        <select id="kanaOperator" name="kanaOperator">
            <option value="Equal" selected="selected">Equal (default)</option>
            <option value="Contains">Contains</option>
        </select>

    </fieldset>
    <fieldset>
        <legend>レリティ</legend>

        <label for="rarity">rarity</label>
        <select id="rarity" name="rarity">
            <option value="Unknown" selected="selected">Unknown (default)</option>
            <option value="Normal">Normal</option>
            <option value="Rare">Rare</option>
            <option value="SRare">SRare</option>
        </select>

        <label for="isPlus">isPlus</label>
        <select id="isPlus" name="isPlus">
            <option value="" selected="selected">(default)</option>
            <option value="true">true</option>
            <option value="false">false</option>
        </select>

    </fieldset>
    <fieldset>
        <legend>その他</legend>

        <label for="idolType">idolType</label>
        <select id="idolType" name="idolType">
            <option value="Unknown" selected="selected">Unknown (default)</option>
            <option value="Cute">Cute</option>
            <option value="Cool">Cool</option>
            <option value="Passion">Passion</option>
            <option value="Trainer">Trainer</option>
        </select>

        <label for="count">count</label>
        <input type="text" id="count" name="count" value="32" />

    </fieldset>
    <input type="submit" value="検索" />
    <dl>
    </dl>
    </form>
    <fieldset>
        <legend>結果</legend>
        <table>
            <thead>
                <tr>
                    <th>画像</th>
                    <th>ヘッドライン</th>
                    <th>名前</th>
                    <th>年齢/血液型/星座</th>
                    <th>身長/体重/3サイズ</th>
                    <th>出身地/趣味/利き手</th>
                </tr>
            </thead>
            <tbody data-bind="foreach: items">
                <tr>
                    <td>
                        <a data-bind="attr: { href: ImageUrl }" href="#" target="_blank">
                            <img src="#" data-bind="attr: { src: IconImageUrl }" />
                        </a>
                        <br />
                        <a data-bind="attr:{ href: FramelessImageUrl }" href="#" target="_blank">FL</a>
                        <a data-bind="attr:{ href: QuestImageUrl }" href="#" target="_blank">QS</a>
                        <a data-bind="attr:{ href: BannerImageUrl }" href="#" target="_blank">LS</a>
                    </td>
                    <td>
                        <span data-bind="text: Rarity"></span>
                        <br />
                        <span data-bind="text: Headline"></span>
                        <br />
                        <span data-bind="text: Hash"></span>
                    </td>
                    <td>
                        <span data-bind="text: Name"></span>
                        <br />
                        <span data-bind="text: Kana"></span>
                        <br />
                        <span data-bind="text: IdolType"></span>
                    </td>
                    <td>
                        <span data-bind="text: Age"></span>
                        <br />
                        <span data-bind="text: BloodType"></span>
                        <br />
                        <span data-bind="text: SunSign"></span>
                    </td>
                    <td>
                                 <span data-bind="text: Height"></span>
                        <br />
                        <span data-bind="text: Weight"></span>
                        <br />
                        <span data-bind="text: Bust"></span>/<span data-bind="text: Waist"></span>/<span data-bind="text: Hip"></span>
                    </td>
                    <td>
                        <span data-bind="text: Birthplace"></span>
                        <br />
                        <span data-bind="text: Hobby"></span>
                        <br />
                        <span data-bind="text: Handedness"></span>
                    </td>
                </tr>
            </tbody>
        </table>
    </fieldset>
    <fieldset>
        <legend>URL</legend>
        <input id="url" type="text" style="min-width: 80em" />
    </fieldset>


    <fieldset>
        <legend>JSON</legend>

        <iframe name="rawJson" style="width: 90%; min-height: 320px; resize: both;"></iframe>
        <script type="text/javascript">


            var vm = {
                items: ko.observableArray()
            };

            ko.applyBindings(vm);

            var f = $('iframe')[0];
            f.onload = function () {
                $('#url').val(f.contentWindow.location.href);

                var d = f.contentWindow.document;

                var json = d.documentElement.innerText;

                var pre = d.createElement('pre');
                pre.style.whiteSpace = 'pre-wrap';
                pre.appendChild(d.createTextNode(json));

                d.documentElement.innerHTML = '';
                d.documentElement.appendChild(pre);

                var re = JSON.parse(json);

                vm.items(re.Items);
            }

        </script>
    </fieldset>
</body>
</html>
