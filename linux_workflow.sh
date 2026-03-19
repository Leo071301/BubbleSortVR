# clone my version of Spatial SDK
git clone https://github.com/seoProductions/io.spatial.unitysdk-1.69.0.git && \
mv  io.spatial.unitysdk-1.69.0  io.spatial.unitysdk@1.69.0
# EVERY THING BELOW is just me telling Unity to use my version of Spatial SDK


# DEPRECIATED
# update manifest.json configuration
#sed -i '/  },/d' Packages/manifest.json
#sed -i '/  "scopedRegistries": \[/d' Packages/manifest.json
#sed -i '/    {/d' Packages/manifest.json
#sed -i '/      "name": "package.openupm.com",/d' Packages/manifest.json
#sed -i '/      "url": "https:\/\/package.openupm.com",/d' Packages/manifest.json
#sed -i '/      "scopes": \[/d' Packages/manifest.json
#sed -i '/        "com.openupm",/d' Packages/manifest.json
#sed -i '/        "io.spatial.unitysdk"/d' Packages/manifest.json
#sed -i '/      ]/d' Packages/manifest.json
#sed -i '/    }/d' Packages/manifest.json
#sed -i '/  ]/d' Packages/manifest.json
#sed -i 's/^.*io.spatial.unitysdk.*$/    "io.spatial.unitysdk": "file:..\/io.spatial.unitysdk@1.69.0",/' Packages/manifest.json

# find & replace scoped repo
linenum=$(awk '/},/ { print NR; exit }' Packages/manifest.json)
sed -i "${linenum},+10d" Packages/manifest.json # delete 10 lines
# insert curly
sed -i "${linenum}i\\  }" Packages/manifest.json

# point SDK to local file
sed -i 's/^.*io.spatial.unitysdk.*$/    "io.spatial.unitysdk": "file:..\/io.spatial.unitysdk@1.69.0",/' Packages/manifest.json


# update packages-lock.json
# from registry to local
linenum=$(awk '/io.spatial.unitysdk/ { print NR; exit }' Packages/packages-lock.json)
linenum=$((linenum + 1)) #  correct position

# remove old setting
sed -i "${linenum},+2d" Packages/packages-lock.json

# settings to tell unity that spatial SDK is local
sed -i "${linenum}i\\      \"version\": \"file:..\/io.spatial.unitysdk@1.70.0\",\n      \"depth\": 0,\n      \"source\": \"local\"," Packages/packages-lock.json

# go to & remove https URL
linenum=$((linenum + 11))
sed -i "${linenum},+1d" Packages/packages-lock.json
# add curly brace
sed -i "${linenum}i\\      }" Packages/packages-lock.json

# just to debug
cat Packages/packages-lock.json
cat Packages/manifest.json
cat ProjectSettings/PackageManagerSettings.asset
ls -al

timeout 18m unity-editor -batchmode -projectPath . -username ${{ secrets.UNITY_EMAIL }} -password ${{ secrets.UNITY_PASSWORD }} -serial ${{ secrets.UNITY_SERIAL }} -SpatialToken  ${{ secrets.SPATIAL_TOKEN_SEOPRODUCTIONS }} -executeMethod SpatialSys.UnitySDK.Editor.BuildUtility.BuildAndPublishPackage -logfile log.txt 
