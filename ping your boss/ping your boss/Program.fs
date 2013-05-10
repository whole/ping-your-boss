// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
#if interactive
#r "System.DirectoryServices.dll"
#endif
open System
open System.Collections
open System.Net.NetworkInformation
open System.Threading.Tasks
open System.Windows.Forms
open System.Drawing

let path = "../hosts.txt"
let readFromFile() = 
    System.IO.File.ReadAllLines(path);;
let populateComboBox(savedHosts: string[]) =
    let cmb = new ComboBox(Size = new Size(100, 16), Dock=DockStyle.Left)     
    for i in savedHosts do
        cmb.Items.Add(i) |> ignore
    cmb

let refreshComboBox(cmb : ComboBox) =
    for i in readFromFile() do
        if (cmb.Items.Contains(i) = false) then
            cmb.Items.Add(i) |> ignore
        cmb.Refresh()
    
[<STAThread>]
do Application.EnableVisualStyles()

let cmb = populateComboBox(readFromFile())
let btnSearch = new Button(Text = "Check your boss", Dock=DockStyle.Right)
let checkAllHosts = new Button(Text = "Fetch all computers", Dock=DockStyle.Top)

let alertLabel = new ToolStripLabel(Dock=DockStyle.Right)
let label = new Label(Dock=DockStyle.Right)
label.Width = 400 |> ignore
alertLabel.BackColor =System.Drawing.Color.Red |> ignore
let toolbarControls = new ToolStrip(Dock=DockStyle.Top)


// Actions 
   (* 1 fetching all hosts *) 
checkAllHosts.Click.Add(fun arg ->   
        let parsedString = Pinging.availableHosts([""])
        System.IO.File.WriteAllLines(path, Pinging.hosts())
        label.Text <- parsedString
        refreshComboBox(cmb)
        )

     (* 2 clicking go button*) 
btnSearch.Click.Add(fun arg ->
         alertLabel.Text <- System.String.Empty
         match cmb.SelectedItem with
             | null -> alertLabel.Text <- "You must select item"
             | _    -> let selected = cmb.SelectedItem.ToString()
                       let parsedString = Pinging.availableHosts([selected])
                       match parsedString with
                            | "" -> label.Text <- selected + " offline"
                            | _ -> label.Text <- parsedString
         )


// adding items to toolbar
toolbarControls.Items.Add(alertLabel)  |>ignore

let form = new Form(Text="Ping your boss!", Size=new Size(400, 400))
form.Controls.Add(cmb)  |>ignore
form.Controls.Add(btnSearch)  |>ignore
form.Controls.Add(checkAllHosts)  |>ignore
form.Controls.Add(label) |> ignore



form.Controls.Add(toolbarControls)

form.PerformLayout()
form.Show()

Application.Run(form)

    // return an integer exit code
