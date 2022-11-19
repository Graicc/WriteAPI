using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteAPI
{
	internal static class Docs
	{
		public const string docs = @"
<!doctype html>
<html>
<head>
  <meta charset=""utf-8""> <!-- Important: rapi-doc uses utf8 charecters -->
  <script type=""module"" src=""https://unpkg.com/rapidoc/dist/rapidoc-min.js""></script>
</head>
<body>
  <rapi-doc
    render-style = ""read"" 
    primary-color = ""#34c7c8"" 
    secondary-color = ""#a94991"" 
    show-header = ""false"" 
    show-info = ""true""
    spec-url = ""/openapi.yaml"" 
    default-schema-tab = 'example'
    > 

    <div slot=""nav-logo"" style=""display: flex; align-items: center; justify-content: center;""> 
      <img src = ""https://community-community-kit.pages.dev/images/logos/arena_disc_logo.png"" style=""width:5em; margin: 1em auto;"" />
    </div>
  </rapi-doc>
</body>
</html>
";

		public const string yaml = @"
openapi: 3.0.1
info:
  title: WriteAPI
  description: A tool for reading and writing various values from the Echo Games memory. This can be used in a similar way to the official API but with far lower latency.
  version: 1.0.0
servers:
  - url: http://127.0.0.1:6723/
tags:
  - name: Echo VR
  - name: Lone Echo
  - name: Lone Echo 2
paths:
  /echovr/camera_transform:
    get:
      tags:
        - Echo VR
      summary: Returns camera position and rotation
      responses:
        '200':
          description: Response while the game is hooked. Camera needs to be in freecam (C)
          content:
            application/json:
              examples:
                SampleResponse:
                  value: |-
                    {
                        ""position"": {
                            ""X"": 0.0,
                            ""Y"": 0.0,
                            ""Z"": 0.0
                        },
                        ""rotation"": {
                            ""X"": 0.0,
                            ""Y"": 0.0,
                            ""Z"": 0.0,
                            ""W"": 0.0
                        }
                    }
    post:
      tags:
        - Echo VR
      summary: Sets camera position and rotation, then returns them
      requestBody:
        description: The desired camera transform
        content:
          'application/json':
            schema:
              $ref: '#/components/schemas/Transform'
        required: true
      responses:
        '200':
          description: Response while the game is hooked. Camera needs to be in freecam (C)
          content:
            application/json:
              examples:
                SampleResponse:
                  value: |-
                    {
                        ""position"": {
                            ""X"": 0.0,
                            ""Y"": 0.0,
                            ""Z"": 0.0
                        },
                        ""rotation"": {
                            ""X"": 0.0,
                            ""Y"": 0.0,
                            ""Z"": 0.0,
                            ""W"": 0.0
                        }
                    }
  /lonecho/speed:
    get:
      tags:
        - Lone Echo
      summary: Returns player speed
      responses:
        '200':
          description: Response while the game is hooked.
          content:
            application/json:
              examples:
                SampleResponse:
                  value: |-
                    {
                        ""speed"": 0.0
                    }
  /lonecho2/speed:
    get:
      tags:
        - Lone Echo 2
      summary: Returns player speed
      responses:
        '200':
          description: Response while the game is hooked.
          content:
            application/json:
              examples:
                SampleResponse:
                  value: |-
                    {
                        ""speed"": 0.0
                    }
                    
components:
  schemas:
    Transform:
      type: object
      properties:
        position:
          type: object
          properties:
            X:
              type: number
            Y:
              type: number
            Z: 
              type: number
              
        rotation:
          type: object
          properties:
            X:
              type: number
            Y:
              type: number
            Z: 
              type: number
            W: 
              type: number
        
";
	}
}
