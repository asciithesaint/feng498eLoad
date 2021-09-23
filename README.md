# feng498eLoad
This is a constant current electronic load which contrelled with a Windows form app gui.

Project report can be read to understand electronic design, gui and firmware but reading just abstract, introduction, methodology and after it is reasonable. Reading between not necessary and recommended because it includes a lot of misinformations.

Furthermore, total design is as simple as it gets. Mosfet sunk the amperage, microcontroller and gui read it through differential opamp configuration and plots the data. Then, they control sunk amperage though another comparator opamp.

Lastly, forgotten to add on the report, empty opamp inputs in LT1014 should be shorted to ground. Oscillated current values(0-50mA) could be resulted by this and/or not-tuned 
feedback of comparator opamp. But, gui works fine it can be used with different designs. Also, non-well fixes on GUI at the end of the report you will see removed from GUI.

## GUI
![](Project%20Pics/5.png)

## Built Load
![](Project%20Pics/1.jpg)

![](Project%20Pics/2.jpg)

![](Project%20Pics/3.jpg)

![](Project%20Pics/4.jpg)
