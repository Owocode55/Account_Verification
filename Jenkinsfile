pipeline {
  agent any
  stages {
    stage('Checkout code') {
      steps {
        git(url: 'https://github.com/Owocode55/Account_Verification', branch: 'master')
      }
    }

    stage('error') {
      steps {
        git(url: 'https://github.com/Owocode55/Account_Verification', branch: 'dev')
      }
    }

  }
}